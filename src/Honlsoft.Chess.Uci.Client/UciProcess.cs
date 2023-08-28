using System.Diagnostics;
using Honlsoft.Chess.Uci.Client.Commands;

namespace Honlsoft.Chess.Uci.Client; 

public class UciProcess {
    private readonly string _executablePath;
    private Process? _currentProcess;
    
    
    public UciProcess(string executablePath) {
        _executablePath = executablePath;

    }

    public void Start() {

        ProcessStartInfo startInfo = new ProcessStartInfo(_executablePath) {
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            CreateNoWindow = true,
        };

        _currentProcess = Process.Start(startInfo);
        
        if (_currentProcess == null) {
            throw new InvalidOperationException("Unable to start process.");
        }
        
        Interface = new StreamUciInputOutput(new UciCommandSerializer(), _currentProcess.StandardOutput, _currentProcess.StandardInput);
    }

    public void Stop() {
        _currentProcess?.Kill(true);
    }
    
    public IUciInputOutput Interface { get; private set; }
}