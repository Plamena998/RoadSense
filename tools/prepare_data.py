"""Подготовка на данните за базата.

Чете суровите файлове от dataSource/ и записва чисти CSV файлове в
dataSource/processed/, които .NET импортерът зарежда в базата.

Стартиране (от корена на репото):
    python tools/prepare_data.py
Нужни пакети: pandas, pyshp, pyproj
"""
import csv
import glob
import os
import re

import pandas as pd
import shapefile
from pyproj import Transformer

ROOT = os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", "dataSource")
OUT = os.path.join(ROOT, "processed")
os.makedirs(OUT, exist_ok=True)


def find(pattern):
    return glob.glob(os.path.join(ROOT, pattern))[0]


def save(df, name):
    df.to_csv(os.path.join(OUT, name), index=False, encoding="utf-8")
    print(f"{name}: {len(df)} реда")


# Областни центрове (същите, за които е изтеглено времето в weather/)
CENTERS = {
    "БЛАГОЕВГРАД": (42.02, 23.09), "БУРГАС": (42.50, 27.47), "ВАРНА": (43.21, 27.91),
    "ВЕЛИКО ТЪРНОВО": (43.08, 25.63), "ВИДИН": (43.99, 22.88), "ВРАЦА": (43.20, 23.55),
    "ГАБРОВО": (42.87, 25.32), "ДОБРИЧ": (43.57, 27.83), "КЪРДЖАЛИ": (41.65, 25.37),
    "КЮСТЕНДИЛ": (42.28, 22.69), "ЛОВЕЧ": (43.14, 24.72), "МОНТАНА": (43.41, 23.23),
    "ПАЗАРДЖИК": (42.19, 24.33), "ПЕРНИК": (42.60, 23.03), "ПЛЕВЕН": (43.42, 24.62),
    "ПЛОВДИВ": (42.14, 24.75), "РАЗГРАД": (43.53, 26.52), "РУСЕ": (43.85, 25.97),
    "СИЛИСТРА": (44.12, 27.26), "СЛИВЕН": (42.68, 26.32), "СМОЛЯН": (41.58, 24.70),
    "СОФИЙСКА": (42.90, 23.79), "СОФИЯ": (42.70, 23.32), "СТАРА ЗАГОРА": (42.43, 25.64),
    "ТЪРГОВИЩЕ": (43.25, 26.57), "ХАСКОВО": (41.93, 25.56), "ШУМЕН": (43.27, 26.93),
    "ЯМБОЛ": (42.48, 26.51),
}

# 1. Области
oblasts = pd.DataFrame(
    [{"name": n, "latitude": la, "longitude": lo} for n, (la, lo) in sorted(CENTERS.items())]
)
save(oblasts, "oblasts.csv")

# 2. ПТП по области и период (2026-H1 и 2025-H1)
src = pd.read_csv(find("tezhki*oblasti*.csv"), encoding="utf-8-sig")
src.columns = ["oblast", "p26", "k26", "i26", "p25", "k25", "i25"] + list(src.columns[7:])
src = src[src.oblast != "Общо"]
rows = []
for _, r in src.iterrows():
    rows.append({"oblast": r.oblast, "period": "2026-H1", "ptp": r.p26, "killed": r.k26, "injured": r.i26})
    rows.append({"oblast": r.oblast, "period": "2025-H1", "ptp": r.p25, "killed": r.k25, "injured": r.i25})
save(pd.DataFrame(rows), "oblast_period_stats.csv")

# 3. ПТП по област и месец (2026)
mon = pd.read_csv(find("spravka*.csv"), encoding="utf-8-sig")
mon = mon[mon["Области"] != "Общо"]
months = ["януари", "февруари", "март", "април", "май", "юни"]
rows = [
    {"oblast": r["Области"], "year": 2026, "month": i + 1, "ptp": r[m]}
    for _, r in mon.iterrows() for i, m in enumerate(months)
]
save(pd.DataFrame(rows), "oblast_monthly_ptp.csv")

# 4. ПТП по час и ден от седмицата (цялата страна).
#    Файлът е счупен: всеки ред е цял в кавички, затова се чисти ръчно.
days = ["понеделник", "вторник", "сряда", "четвъртък", "петък", "събота", "неделя"]
rows = []
with open(find("tezhki*chasovi*.csv"), encoding="utf-8-sig") as f:
    lines = [l.strip() for l in f if l.strip()]
for line in lines[1:]:
    cells = next(csv.reader([line.strip('"').replace('""', '"')]))
    if cells[0] == "Общо":  # общ ред - не се импортира, ползва се само за проверка
        totals = [int(x) for x in cells[1:]]
        continue
    hour = int(re.match(r"От (\d\d)", cells[0]).group(1))
    vals = [int(x) for x in cells[1:]]
    for d in range(7):  # за всеки ден: ПТП, загинали, ранени
        rows.append({"weekday": d + 1, "hour": hour,
                     "ptp": vals[d * 3], "killed": vals[d * 3 + 1], "injured": vals[d * 3 + 2]})
hw = pd.DataFrame(rows)
assert (hw.ptp.sum(), hw.killed.sum(), hw.injured.sum()) == tuple(totals[-3:]), "сборът не съвпада с реда Общо"
save(hw, "hour_weekday_stats.csv")

# 5. Времето - от почасови на дневни стойности
rows = []
for f in glob.glob(os.path.join(ROOT, "weather", "*.csv")):
    name = os.path.basename(f)[:-4]
    w = pd.read_csv(f, skiprows=3)
    w.columns = ["time", "temp", "hum", "prec", "rain", "snow", "snowd", "wcode", "cloud", "wind", "gust", "isday"]
    w["date"] = w.time.str[:10]
    d = w.groupby("date").agg(
        temp_min=("temp", "min"), temp_max=("temp", "max"), temp_mean=("temp", "mean"),
        precipitation_mm=("prec", "sum"), snowfall_cm=("snow", "sum"),
        humidity_mean=("hum", "mean"), cloud_mean=("cloud", "mean"),
        wind_max_kmh=("wind", "max"), gust_max_kmh=("gust", "max"), weather_code_max=("wcode", "max"),
    ).reset_index()
    d.insert(0, "oblast", name)
    rows.append(d.round(2))
save(pd.concat(rows), "weather_daily.csv")

# 6. Мостове и тунели (UTM 35N -> WGS84); km е в метри -> километри
tr = Transformer.from_crs("EPSG:32635", "EPSG:4326", always_xy=True)
for folder, shp, out in [("bridge_point", "RI_Bridge_point", "bridges.csv"),
                         ("tunnels_point", "RI_Tunnel_point", "tunnels.csv")]:
    r = shapefile.Reader(os.path.join(ROOT, "gis", folder, shp))
    rows = []
    for rec, sh in zip(r.records(), r.shapes()):
        lon, lat = tr.transform(sh.points[0][0], sh.points[0][1])
        rows.append({"road_ref": rec["RoadID"], "km": round(rec["km"] / 1000, 3),
                     "latitude": round(lat, 6), "longitude": round(lon, 6)})
    save(pd.DataFrame(rows), out)

# 7. Източници на данни (за етикетите в сайта)
sources = [
    ("МВР – тежки ПТП по области", "01.01.2026–30.06.2026 и 01.01.2025–30.06.2025", "МВР (файлове в dataSource/)"),
    ("МВР – тежки ПТП по области и месеци", "01.01.2026–30.06.2026", "МВР (файлове в dataSource/)"),
    ("МВР – тежки ПТП по часове и дни от седмицата", "01.01.2026–30.06.2026", "МВР (файлове в dataSource/)"),
    ("Open-Meteo – архив на времето", "01.01.2025–30.06.2026", "https://open-meteo.com/en/docs/historical-weather-api"),
    ("АПИ – ГИС данни (мостове, тунели)", "2026", "https://api.bg/bg/gis.html"),
]
save(pd.DataFrame(sources, columns=["name", "period", "url"]), "data_sources.csv")
