CREATE TABLE "CandlestickChart"
(
    "Timestamp"       INTEGER NOT NULL UNIQUE,
    "OpeningPrice"    REAL    NOT NULL,
    "ClosingPrice"    REAL    NOT NULL,
    "HighestPrice"    REAL    NOT NULL,
    "LowestPrice"     REAL    NOT NULL,
    "GeneratedVolume" REAL    NOT NULL,
    PRIMARY KEY ("Timestamp")
);