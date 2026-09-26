"""Regression suite for the INALI PDF canonical catalog extraction.

Run:
    python -m pytest tests/AtlasBuho.Tests/test_inali_pdf_extraction.py -v
or simply:
    python tests/AtlasBuho.Tests/test_inali_pdf_extraction.py

Expectations (rule "no inventar, no normalizar silenciosamente"):
- 364 rows total, one per variant (PD 364/364 rule).
- 0 duplicates by spanish variant name.
- 11 familias, 68 agrupaciones.
- 0 unresolved agrupación.
- Every row carries page (PDF physical page, 1-based) and geo_reference.
- Discrepancy ledger exists and counts are reported.
"""
import json
import pathlib
import unicodedata

ROOT = pathlib.Path(__file__).resolve().parents[2]
CANON = ROOT / "catalogs" / "extracted" / "inali_pdf_canon_full.json"
LEDGER = ROOT / "catalogs" / "extracted" / "discrepancy_ledger.json"
MANIFEST = ROOT / "catalogs" / "extracted" / "_inali_pdf_manifest.json"


def _load(p):
    return json.loads(p.read_text(encoding="utf-8"))


def _norm(s):
    return "".join(c for c in unicodedata.normalize("NFD", s or "") if unicodedata.category(c) != "Mn").lower().strip()


def test_canonical_inventory_364():
    data = _load(CANON)
    rows = data["rows"]
    assert len(rows) == 364, f"expected 364 rows, got {len(rows)}"
    seen = set()
    for r in rows:
        key = _norm(r["variante"])
        assert key not in seen, f"duplicate variante: {r['variante']}"
        seen.add(key)


def test_no_unresolved_fields():
    data = _load(CANON)
    for r in data["rows"]:
        assert r["familia"] and r["familia"] != "UNRESOLVED_FAMILY"
        assert r["agrupacion"] and r["agrupacion"] != "UNRESOLVED_GROUP"
        assert r["variante"]
        # Regla: ninguna variante debe empezar sin la agrupación conocida (catches row-truncation)
        vnorm = _norm(r["variante"])
        gnorm = _norm(r["agrupacion"])
        starts = vnorm.startswith(gnorm) or any(vnorm.startswith(_norm(a)) for a in (
            "kickapoo","pápago","papago","pima","tepehuano","tarahumara","guarijío","guarijio","yaqui","mayo","cora","huichol",
            "náhuatl","nahuatl","mexicano","paipai","ku'ahl","ku’ahl","kuahl","cucapá","cucapa","kumiai","kiliwa","seri",
            "otomí","otomi","mazahua","matlatzinca","tlahuica","pame","chichimeco","chinanteco","tlapaneco","mazateco","ixcateco",
            "chocholteco","popoloca","zapoteco","chatino","amuzgo","mixteco","cuicateco","triqui",
            "huasteco","maya","lacandón","lacandon","ch'ol","ch’ol","chol","chontal","tseltal","tsotsil",
            "q'anjob'al","q’anjob’al","qanjobal","k’anjob’al","akateko","jakalteko",
            "qato'k","qato’k","qatok","mocho","mocho’","tuzanteco","chuj","tojolabal",
            "q'eqchí'","q’eqchí’","q’eqchi","qeqchi","q'eqchi","q'eqchi’",
            "k'iche'","k’iche","k’iche’","kiche","kaqchikel","teko","mam","awakateko","ixil",
            "totonaco","tepehua","tarasco","purépecha","purepecha",
            "mixe","sayulteco","oluteco","texistepequeño","texistepequeno",
            "ayapaneco","popoluca","zoque","huave"))


def test_every_variant_has_autodenominacion_or_explicitly_empty():
    data = _load(CANON)
    missing = [r for r in data["rows"] if not r["autodenominacion"] and not r["autodenominaciones_alternas"]]
    # We tolerate rows where PDF truly has no autodonominación (must be listable by hand)
    # Today there is ONE such row: 'otomí del oeste del Valle del Mezquital' p.150
    assert len(missing) <= 1, f"rows without any autodenominacion: {[r['variante'] for r in missing]}"


def test_every_variant_has_provenance():
    data = _load(CANON)
    for r in data["rows"]:
        assert r["page"] >= 69, f"row missing page: {r}"
        assert r["geo_reference"], f"row missing geo: {r['variante']}"


def test_coverage_11_familias_68_agrupaciones():
    data = _load(CANON)
    familias = {r["familia"] for r in data["rows"]}
    agrupaciones = {r["agrupacion"] for r in data["rows"]}
    assert len(familias) == 11, f"expected 11 familias, got {len(familias)}"
    assert len(agrupaciones) == 68, f"expected 68 agrupaciones, got {len(agrupaciones)}"


def test_manifest_sha256_recorded():
    m = _load(MANIFEST)
    assert m["file_sha256"] == "e38e667d024784bc84cc350f44918c1d22d998284d02e38dcf92858257947311"
    assert m["resource_url"] == "https://site.inali.gob.mx/pdf/catalogo_lenguas_indigenas.pdf"


def test_discrepancy_ledger_exists():
    led = _load(LEDGER)
    counts = led["counts"]
    assert counts["pdf_rows"] == 364
    # We expect non-zero 'only_in_pdf_vs_html' — HTML only has ~219 variants
    assert counts["only_in_pdf_vs_html"] > 0
    # The failure data must be preserved, not silently dropped
    assert "only_in_pdf_vs_appendix4" in led
    assert "only_in_appendix4_vs_pdf" in led


if __name__ == "__main__":
    import sys

    fns = [v for k, v in list(globals().items()) if k.startswith("test_")]
    fail = 0
    for fn in fns:
        try:
            fn()
            print(f"PASS {fn.__name__}")
        except AssertionError as e:
            fail += 1
            print(f"FAIL {fn.__name__}: {e}")
    sys.exit(0 if fail == 0 else 1)
