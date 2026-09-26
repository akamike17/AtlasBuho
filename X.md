# AtlasBúho — PHASE 1 SURGICAL CORRECTION

## Provenance Integrity / Reconciliation / Idempotency / Ambiguity Gate

**Repository:** `akamike17/AtlasBuho`
**Current branch:** `review/phase-1-inventario-linguistico`
**Current commit:** `0023b92`
**Parent:** `313bfe5`

## MISSION

Do NOT start a new feature phase.

Do NOT rewrite the architecture.

Do NOT merely update audit documentation.

This is a **surgical correction pass over commit `0023b92`**.

The objective is to make the Phase 1 linguistic inventory importer **documentarily deterministic, provenance-safe, ambiguity-safe, idempotent, reproducible and auditable**.

The repository must not claim `CLOSED`, `VERIFIED`, `PROVENANCE INTACT`, or equivalent until the required evidence below is actually produced.

---

# 1. HARD RULES

1. Never invent linguistic data.
2. Never silently discard a source row.
3. Never choose an arbitrary candidate when a linguistic identity is ambiguous.
4. Never use a fake/default source page to make provenance appear complete.
5. Never use `FirstOrDefault()` as a semantic resolver when multiple candidates are possible.
6. Preserve original source values separately from normalized comparison values.
7. Preserve quarantine evidence beyond process memory.
8. Do not weaken tests to make the suite green.
9. Do not modify source PDFs or source JSON merely to make counts pass.
10. Do not change documentary baselines without documentary evidence.
11. Do not declare a quarantined row equivalent to a successfully resolved row.
12. Do not convert an unresolved ambiguity into a guessed association.
13. Do not hide missing `RawTextHash`.
14. Do not claim MySQL Unicode fidelity without a real MySQL round-trip test.
15. Do not claim idempotency because one run succeeds; prove repeated execution.

---

# 2. CURRENT CONFIRMED PROBLEMS

The following issues were confirmed by inspection of `0023b92`.

## H-101 — SourcePage.RawTextHash is still NULL

`CreateOrGetSourceDocumentAsync()` creates `SourcePage` records but assigns:

```csharp
rawTextHash: null
```

The comments explicitly say the hash will be calculated later, but the current importer does not perform that operation.

### Required correction

Implement deterministic page-level text extraction and SHA256 hashing.

Requirements:

* Use the repository's established extraction mechanism where available.
* Do not silently fall back to `null`.
* Normalize text only according to a documented deterministic rule.
* Preserve enough information to reproduce the hash.
* Calculate hashes for every page represented by the provenance model.
* Persist the hashes.
* Add tests proving the same source PDF produces the same page hashes.

If page extraction cannot be performed for a page:

* create an explicit extraction failure/quarantine record;
* do NOT write `NULL` and call provenance complete.

---

# 3. H-102 — NO DEFAULT SOURCE PAGE ALLOWED

Current code contains fallback behavior equivalent to:

```csharp
agrupacionToPage.GetValueOrDefault(groupName, 244)
```

and:

```csharp
variantToPage.GetValueOrDefault(variant.Name, 30)
```

This is prohibited.

A provenance system must never convert:

> "I don't know the source page"

into:

> "page 30/244"

### Required correction

Replace every provenance fallback with explicit resolution.

Allowed states:

* `Resolved`
* `Quarantined`
* `Unresolved`

Never:

* guessed page
* default page
* arbitrary first page
* index page used as substitute for actual evidence

For every `CatalogRecord`:

```text
SourceDocumentId
SourcePage
SourceSection
SourceHash
ParserVersion
ResolutionMethod
```

must correspond to actual evidence.

If a family/group does not have an individually resolved page, model that explicitly rather than fabricating one.

---

# 4. H-103 — REMOVE AMBIGUOUS FIRST-CANDIDATE SELECTION

Current importer contains logic equivalent to:

```text
spanish_name_prefix_ambiguous_first
```

and chooses:

```text
prefixMatches[0]
```

This is unacceptable for AtlasBúho.

### Required behavior

If:

```text
candidate count == 0
```

→ unresolved/quarantine.

If:

```text
candidate count == 1
```

→ resolve.

If:

```text
candidate count > 1
```

→ apply deterministic evidence-backed disambiguation only.

Valid disambiguators may include:

* exact source-page relationship;
* explicit location present in source;
* exact autodenomination/context relationship;
* documented canonical mapping;
* unique family + grouping + source-page identity.

If ambiguity remains:

```text
QUARANTINE
```

Never select the first candidate.

Add a regression test specifically proving that an ambiguous set cannot resolve automatically.

---

# 5. H-104 — PRESERVE ORIGINAL VS NORMALIZED VALUES

Current code performs normalization such as:

```csharp
Replace("\n", " ")
Replace("\r", " ")
Trim()
```

That is acceptable for comparison but must not destroy source fidelity.

### Required model/logic

Maintain conceptually:

```text
OriginalSourceValue
NormalizedComparisonValue
```

The original extracted value must remain recoverable.

Normalization must be used only for matching/indexing unless the source itself contains the normalized value.

Add tests covering:

* accented characters;
* apostrophes;
* hyphens;
* parentheses;
* Unicode combining characters;
* newline artifacts;
* whitespace variation.

---

# 6. H-105 — QUARANTINE MUST BE PERSISTENT

Current:

```csharp
ImportResult.QuarantinedItems
```

is in-memory.

That is insufficient for a forensic linguistic catalog.

### Required correction

Create or use a persistent audit/quarantine entity/table.

Each quarantined row must preserve at minimum:

```text
Id
CatalogVersionId
SourceDocumentId
EntityType
RawData
SourcePage
SourceSection
Reason
ResolutionMethod
CreatedAt
Status
```

Recommended status:

```text
Open
Resolved
AcceptedAsArtifact
Rejected
```

A failed import must not destroy quarantine evidence.

The quarantine record must be associated with the exact CatalogVersion/source document.

---

# 7. H-106 — DOCUMENTARY COUNT ACCOUNTING MUST BE EXPLICIT

Current baseline logic allows:

```text
imported + quarantined = documentary baseline
```

This is acceptable only if the quarantine classifications are individually persisted and auditable.

The system must report separately:

```text
Source rows
Successfully resolved
Quarantined unresolved
Quarantined artifact
Excluded header
Rejected invalid
```

For variants:

```text
364 source
X resolved
Y quarantined
X + Y = 364
```

For valid Appendix 4 autodenominations:

```text
474 valid source rows
X resolved
Y unresolved
X + Y = 474
```

Artifacts must remain separate:

```text
3 source artifacts
3 artifact-classified
0 silently discarded
```

No category may disappear between parsing and persistence.

---

# 8. H-107 — AUTODENOMINATION MATCHING MUST BE FORENSICALLY DETERMINISTIC

The current importer uses several heuristics:

* exact prefix;
* progressive prefix;
* suffix;
* substring;
* location;
* other fallback methods.

These may remain only if each resolution produces:

```text
ResolutionMethod
CandidateSet
SelectedCandidate
Evidence
ConfidenceClass
```

Do NOT introduce an ML guesser here.

This phase is source reconciliation, not translation.

Recommended resolution classes:

```text
EXACT_SOURCE_MATCH
UNIQUE_PREFIX_MATCH
UNIQUE_LOCATION_MATCH
EXACT_CONTEXT_MATCH
DETERMINISTIC_COMPOSITE_MATCH
AMBIGUOUS
UNRESOLVED
ARTIFACT
```

Anything ambiguous or unresolved must be quarantined.

---

# 9. H-108 — CATALOG RECORDS MUST CARRY ACTUAL SOURCE EVIDENCE

Current variant records set:

```text
geoReference = null
```

even though the source JSON contains geo-reference data.

Do not populate fields merely because they exist, but if the source field is available and belongs to the record, propagate it.

Likewise, do not manufacture descriptions, INALI codes, ISO codes, or linguistic metadata.

The principle is:

```text
source field available → preserve it
source field unavailable → NULL with explicit provenance state
never fabricate
```

---

# 10. H-109 — FAMILY/GROUP PROVENANCE MUST BE EXPLICIT

Do not use:

```text
sourcePage = 30
```

merely because page 30 is an index.

If the source evidence is an index entry, label it as such:

```text
SourceSection = Index
EvidenceType = IndexReference
```

If the actual linguistic grouping occurs elsewhere, preserve that page separately.

Do not conflate:

```text
index reference
```

with:

```text
entity source page
```

---

# 11. H-110 — IDENTITY MUST INCLUDE DOCUMENTARY CONTEXT

Current variant lookup effectively relies on:

```text
LanguageGroupId + normalized Name
```

That is not enough to establish documentary identity across versions.

Define and document a deterministic logical identity incorporating the source context.

At minimum evaluate:

```text
SourceDocumentHash
Family
Group
OriginalName
SourcePage
```

Do NOT blindly add a database unique index until existing duplicate semantics have been analyzed.

The goal is:

> same source + same documentary entity → same identity

and:

> different documentary source/version → distinguishable identity

---

# 12. H-111 — IDEMPOTENCY MUST BE PROVEN

Run the importer against a clean MySQL database.

Then:

### Run 1

Record:

* row counts;
* identities;
* CatalogVersion;
* quarantine;
* CatalogRecords;
* SourcePages.

### Run 2

Run the exact same importer again.

Expected:

* no duplicated logical entities;
* no duplicated autodenominations;
* no duplicated CatalogRecords;
* no duplicated quarantine records for the same immutable source/version unless the model explicitly versions import attempts;
* deterministic counts;
* deterministic logical identities.

If a new import execution record is intentionally created, document why.

The second execution must not mutate the semantic dataset.

---

# 13. H-112 — CATALOG VERSION MUST NOT PRETEND SUCCESS

The current workflow:

```text
Pending
→ import
→ validation
→ Completed
```

is acceptable only if all mandatory gates pass.

Required states:

```text
Pending
Importing
Completed
Failed
```

A failed validation must result in:

```text
Failed
```

and must not leave a semantically successful version.

Transaction rollback must be verified.

---

# 14. H-113 — SOURCE HASH VALIDATION

The PDF SHA256 validation currently exists and is good.

Keep it.

Additionally verify:

* file exists;
* size matches stored metadata;
* page count is verified against actual document;
* page extraction succeeds for required pages;
* parsed JSON hashes are verified before import if those JSON files are treated as immutable source artifacts.

Do not trust only a database-stored hash.

---

# 15. H-114 — PARSED JSON MUST HAVE ITS OWN PROVENANCE

If:

```text
inali_final_catalog.json
appendix4_parsed.json
```

are inputs to the importer, record their hashes.

The provenance chain should be:

```text
Official PDF
   ↓ SHA256
Parsed artifact
   ↓ SHA256
Parser version
   ↓
Importer
   ↓
CatalogVersion
   ↓
Entity
   ↓
CatalogRecord
```

Do not claim:

```text
PDF → CatalogRecord
```

if the actual implementation also depends on parsed JSON artifacts without recording them.

---

# 16. H-115 — AUDIT DOCUMENTS MUST MATCH CODE

The current Pass 1 / Pass 2 documents contain statements describing issues that the commit claims to have fixed.

After correction:

1. Re-run the audit.
2. Update the audit documents.
3. Preserve the historical findings.
4. Clearly distinguish:

   * original finding;
   * correction;
   * verification evidence;
   * remaining limitation.

Do not overwrite history as if the original issue never existed.

---

# 17. H-116 — TEST MATRIX REQUIRED

Add or extend tests for:

### Source integrity

* PDF hash mismatch fails.
* Parsed JSON hash mismatch fails.
* page extraction failure fails/quarantines explicitly.

### Unicode

* á
* é
* í
* ó
* ú
* ü
* ñ
* apostrophes
* combining Unicode
* indigenous-language orthography examples present in source.

### Matching

* exact match;
* unique prefix;
* ambiguous prefix;
* progressive prefix;
* location disambiguation;
* unresolved candidate;
* artifact row.

### Provenance

* real page;
* source section;
* source hash;
* parser version;
* parsed-artifact hash;
* no page 30/244 fallback.

### Quarantine

* persistence;
* source linkage;
* reason;
* raw source data;
* deterministic resolution method.

### Idempotency

* Run twice;
* compare logical identity sets;
* compare counts;
* compare CatalogRecords;
* compare quarantine.

### Rollback

Force an import failure and prove:

```text
no partial semantic dataset remains
```

---

# 18. REQUIRED AUDIT COMMANDS / EVIDENCE

Do not report only:

```text
Build PASS
Tests PASS
```

Provide actual evidence for:

```text
dotnet build -c Release
dotnet test
```

and the real MySQL import.

For the importer, report:

```text
Source families
Source groups
Source variants
Source valid autodenominations
Source artifacts

Resolved families
Resolved groups
Resolved variants
Resolved autodenominations

Quarantined variants
Quarantined autodenominations
Quarantined artifacts

Persisted SourcePages
SourcePages with RawTextHash
SourcePages missing RawTextHash

CatalogRecords
CatalogRecords with valid provenance
CatalogRecords with unresolved provenance

First run counts
Second run counts
Semantic delta between runs
```

---

# 19. STOP CONDITIONS

Do NOT claim Phase 1 closed if any of the following remain:

* arbitrary candidate selection;
* default source page;
* null RawTextHash for required provenance;
* silently discarded source rows;
* non-persistent quarantine;
* unproven idempotency;
* undocumented ambiguity;
* failed Unicode round-trip;
* undocumented parsed-artifact dependency;
* code/documentation contradiction.

---

# 20. FINAL REPORT FORMAT

Return a forensic report with these exact sections:

```text
1. EXECUTIVE RESULT
2. FILES CHANGED
3. ROOT CAUSE ANALYSIS
4. CORRECTIONS IMPLEMENTED
5. TEST EVIDENCE
6. MYSQL EVIDENCE
7. PROVENANCE EVIDENCE
8. QUARANTINE EVIDENCE
9. IDEMPOTENCY EVIDENCE
10. UNRESOLVED ITEMS
11. REGRESSIONS
12. FINAL GATE
```

For every issue classify:

```text
CONFIRMED FIXED
CONFIRMED OPEN
NOT APPLICABLE
UNKNOWN
```

Do not use vague terms such as:

```text
looks good
should work
probably fixed
production ready
complete
```

unless accompanied by reproducible evidence.

---

# 21. FINAL GATE

The only acceptable final statuses are:

```text
PHASE 1 — VERIFIED
```

or:

```text
PHASE 1 — CORRECTIONS REQUIRED
```

Do not use:

```text
CLOSED
```

unless every mandatory evidence item above has been independently demonstrated.

The objective is not to make the report look finished.

The objective is to make the **repository itself defensibly correct**.
