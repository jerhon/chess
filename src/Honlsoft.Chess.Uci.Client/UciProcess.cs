using System.Diagnostics;
using Honlsoft.Chess.Uci.Client.Commands;

namespace Honlsoft.Chess.Uci.Client; 

public class UciProcess(string executablePath) {
    
    private Process? _currentProcess;

    public void Start() {

        ProcessStartInfo startInfo = new ProcessStartInfo(executablePath); /* {
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            CreateNoWindow = true,
        };*/

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