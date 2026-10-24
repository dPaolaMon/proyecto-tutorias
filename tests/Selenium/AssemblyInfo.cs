using Xunit;

// Deshabilitar la ejecución paralela entre clases de pruebas de Selenium
// para evitar contención de recursos en el navegador y colisiones de sockets en WebDriver
[assembly: CollectionBehavior(DisableTestParallelization = true, MaxParallelThreads = 1)]
