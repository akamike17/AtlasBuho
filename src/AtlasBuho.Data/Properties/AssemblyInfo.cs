using System.Runtime.CompilerServices;

// The reconciliation and matching internals of the INALI importer are exercised directly by the
// test suite (5B.md FASE 14): the resolution logic and the per-row accounting invariants must be
// testable without going through a live database.
[assembly: InternalsVisibleTo("AtlasBuho.Tests")]
