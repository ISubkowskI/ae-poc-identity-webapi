using Xunit;

// Disable parallel execution for integration tests to prevent Serilog static logger conflicts
[assembly: CollectionBehavior(DisableTestParallelization = true)]
