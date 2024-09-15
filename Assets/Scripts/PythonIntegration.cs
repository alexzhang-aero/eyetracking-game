using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class PythonLauncher : MonoBehaviour
{
    private Process pythonProcess;

    void Start()
    {
        RunPythonScript();
    }

    void RunPythonScript()
    {
        try
        {
            // Get the path to the Python script relative to the Unity project
            string projectRoot = Application.dataPath;  // Points to the Assets folder
            string scriptPath = Path.Combine(projectRoot, "..", "main.py");  // Adjust if necessary

            // Determine the Python command depending on the platform
            string pythonCommand = @"C:\Users\ricar\miniconda3\envs\eyetracking\python.exe";  // Adjust this path


            if (string.IsNullOrEmpty(pythonCommand))
            {
                UnityEngine.Debug.LogError("Could not determine Python command.");
                return;
            }

            // Launch Python script
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = pythonCommand,
                Arguments = $"\"{scriptPath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            pythonProcess = new Process
            {
                StartInfo = startInfo
            };

            pythonProcess.OutputDataReceived += (sender, args) => UnityEngine.Debug.Log(args.Data);
            pythonProcess.ErrorDataReceived += (sender, args) => UnityEngine.Debug.LogError(args.Data);

            pythonProcess.Start();
            pythonProcess.BeginOutputReadLine();
            pythonProcess.BeginErrorReadLine();

            UnityEngine.Debug.Log("Python script started.");
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"Error starting Python script: {ex.Message}");
        }
    }

    string GetPythonCommand()
    {
        string pythonCommand = null;

        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            pythonCommand = "python";  // Assumes Python is in the PATH
        }
        else if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
        {
            pythonCommand = "python3";  // Python is usually installed as python3 on macOS
        }
        else if (Application.platform == RuntimePlatform.LinuxPlayer || Application.platform == RuntimePlatform.LinuxEditor)
        {
            pythonCommand = "python3";  // Python is usually installed as python3 on Linux
        }

        return pythonCommand;
    }

    void OnApplicationQuit()
    {
        // Kill the Python process when Unity quits
        if (pythonProcess != null && !pythonProcess.HasExited)
        {
            pythonProcess.Kill();
            pythonProcess.WaitForExit();  // Ensure the process exits
            UnityEngine.Debug.Log("Python script stopped.");
        }
    }
}
