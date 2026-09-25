# AtlasBuho — B10 Architectural Audit

**Baseline:** `b9484a2`
**Date:** 2026-09-25
**Scope:** Surgical AI separation audit and V2 pipeline design

---

## A. Current Authoritative Path

```text
User Input (text, source lang, target lang)
    ↓
Application/Translation/TranslationRequest (record)
    ↓
Data/Translation/DictionaryTranslationEngine
    ↓
Data/Repositories/{LexemeRepository, LexicalEquivalenceRepository, ...}
    ↓
Domain entities:
    Lexeme (canonical forms, variants)
    Meaning (Spanish/English meanings)
    LexicalEquivalence (V1 evidence: canonical, directional, versioned)
    CatalogVersion (immutable, provenance)
    Source (evidence provenance)
    LanguageVariant (ISO hierarchy)
    ↓
TranslationResult (translated text, provenance, confidence from VerificationStatus)
```

**Verification Status flow:**
- `Verified` / `Documented` → authoritative translations
- `CommunityVerified` / `AcademicVerified` → weighted evidence
- No AI path exists — all translations derive from documentary evidence only

---

## D. Current AI Path (Implemented)

```text
User Request
    ↓
Application/AiReview/IAiTranslationReviewer
    ↓
Infrastructure/AiProviders/MockAiTranslationReviewer (testable, no external deps)
    ↓
Structured AiTranslationReviewResult (Application Layer)
    ↓
Domain/AiReview/AiTranslationReview (persisted, non-canonical)
    ↓
Data/AiTranslationReviewConfiguration (EF Core)
    ↓
AiTranslationReviewRepository (audit log)

V2 Pipeline:
AiTranslationReview (ReviewStatus=LikelyIncorrect/PotentialIssue)
    ↓
Human Review (UNDER_REVIEW → ACCEPTED)
    ↓
V2Candidate (extracted, not promoted)
    ↓
Explicit Promotion (requires evidence verification, out of scope for B10)
```

**Key architectural decisions:**
- `AtlasBuho.Domain/AiReview/` — entities are domain concepts (audit records)
- `AtlasBuho.Application/AiReview/` — `IAiTranslationReviewer` abstraction
- `AtlasBuho.Infrastructure/AiProviders/` — provider implementations (Mock for testing)
- `AtlasBuho.Data/Configurations/` — EF Core mapping (no canonical table references)
- `AtlasBuho.Data/Migrations/` — `20260925173851_AddAiReviewTables`

---

## E. Collision Points (Prevented)

| Risk | Prevention | Evidence |
|------|-----------|----------|
| AI creates canonical evidence | No FKs to canonical tables; separate persistence | `AiTranslationReviewConfiguration` has no entity references to Lexeme/Meaning/LexicalEquivalence |
| Provider coupling in domain | Application-layer abstraction `IAiTranslationReviewer` | Domain has no provider SDK references |
| AI promotion without review | Lifecycle enum: PENDING → UNDER_REVIEW → ACCEPTED/REJECTED → PROMOTED | `AiCandidateLifecycle` with explicit states |
| Language ID overwrite | `DetectedLanguage` stored separately from `DocumentedVariant` | Test3 verifies variant unchanged |
| Secret leakage | No API key fields in entities; env vars only | Test8 verifies no secrets in metadata |

---

## F. Implementation Summary

| Component | File | Purpose |
|-----------|------|---------|
| Domain Entity | `src/AtlasBuho.Domain/AiReview/AiTranslationReview.cs` | Audit record (non-canonical) |
| Domain Entity | `src/AtlasBuho.Domain/AiReview/V2Candidate.cs` | V2 candidate lifecycle |
| Abstraction | `src/AtlasBuho.Application/AiReview/IAiTranslationReviewer.cs` | Provider contract |
| DTOs | `src/AtlasBuho.Application/AiReview/*.cs` | Request/Result/Options |
| Mock Provider | `src/AtlasBuho.Infrastructure/AiProviders/MockAiTranslationReviewer.cs` | Testable AI (no external deps) |
| Service | `src/AtlasBuho.Infrastructure/AiProviders/AiTranslationReviewService.cs` | Orchestration |
| Persistence | `src/AtlasBuho.Data/Configurations/AiReviewConfigurations.cs` | EF Core mapping |
| Migration | `src/AtlasBuho.Data/Migrations/20260925173851_AddAiReviewTables.cs` | Schema |
| Tests | `tests/AtlasBuho.Tests.Integration/AiSeparationTests.cs` | 8 B10 test categories |

---

## G. Gate Compliance (Final)

| Gate | Status | Evidence |
|------|--------|----------|
| A (Repository) | **PASS** | Baseline b9484a2, no unrelated projects |
| B (Architecture) | **PASS** | AI separated; provider abstraction exists; Domain no provider SDK |
| C (Evidence) | **PASS** | No AI in V1 path; doc evidence remains authoritative |
| D (Persistence) | **PASS** | AI reviews persisted; prompt/model/provider recorded; no secrets |
| E (V2) | **PASS** | Corrections → candidates with lifecycle; no auto-promotion |
| F (UI) | **N/A** | No UI project in repo |
| G (Tests) | **PASS** | 20/20 integration (AI), 68/69 unit, 1 skip preexisting |
| H (Build) | **PASS** | Release build 0 errors (19 warnings preexisting) |
| I (Git) | **PASS** | Migration + code committed |

**All applicable gates: PASS**

---

## H. Self-Audit Verification (B10 §16)

| Check | Status | Notes |
|-------|--------|-------|
| Missing translation evidence | N/A | V1 unchanged |
| Duplicate canonical entries | N/A | V1 unchanged |
| Contradictory translations | N/A | V1 unchanged |
| Impossible directionality | N/A | V1 unchanged |
| Variant contamination | **PASS** | Test3 verifies no overwrite |
| Missing provenance | N/A | V1 unchanged |
| Undocumented confidence | **PASS** | `DetectedLanguageConfidence` persisted |
| Invalid verification states | **PASS** | Only `AiReviewStatus` used |

**Persistence checks:**
- AI records do not mutate canonical evidence: **PASS** (Test1)
- Foreign keys correct: **PASS** (V2Candidate → AiTranslationReview, RESTRICT)
- Immutable catalog versions protected: **PASS** (existing triggers unchanged)
- AI logs auditable: **PASS** (all fields persisted)
- Deletion behavior: **PASS** (no cascade to canonical)

---

## Post-Implementation Addendum (2026-09-25)

**Test9_AiReviewDoesNotContaminateCanonicalData added:**
- Proves end-to-end flow: V1 evidence → AI review (proposes "hogar") → canonical unchanged ("casa" remains)
- Verifies V2Candidate NOT promoted (IsPromoted=false, Lifecycle=Pending)
- Asserts "hogar" does NOT exist in LexicalEquivalences (content check, not just count)
- Confirms CatalogVersion remains "Completed" (immutable)

**Results:**
- Integration tests: 21/21 PASS (added Test9)
- Unit tests: 68/69 PASS, 1 skip preexisting (W1HContextRepositoryTests.GetByEntityAsync - InMemory provider limitation)

**Skip explanation:** `GetByEntityAsync_ShouldReturnContextsLinkedToEntity` skipped due to InMemory database query limitation with EntityType filtering (EF Core known issue). Preexisting since Phase 2, not introduced by B10.

*Audit complete. All B10 requirements implemented and verified.*

