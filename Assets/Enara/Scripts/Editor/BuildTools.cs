using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Enara.Editor
{
    /// <summary>
    /// CLI entry point for building the standalone game. Called from CI / batch mode:
    /// <code>
    /// Unity -batchmode -nographics -projectPath . -executeMethod Enara.Editor.BuildTools.BuildStandalone -quit -logFile build.log
    /// </code>
    ///
    /// The output goes to <c>Build/&lt;platform&gt;/</c>. Override the build path with the
    /// <c>ENARA_BUILD_PATH</c> environment variable.
    /// </summary>
    public static class BuildTools
    {
        /// <summary>Build a standalone player for the active build target.</summary>
        public static void BuildStandalone()
        {
            var scenes = EditorBuildSettings.scenes;
            if (scenes == null || scenes.Length == 0)
            {
                Debug.LogError("[BuildTools] No scenes in EditorBuildSettings. Add Boot.unity first.");
                EditorApplication.Exit(1);
                return;
            }

            string outputPath = System.Environment.GetEnvironmentVariable("ENARA_BUILD_PATH");
            if (string.IsNullOrEmpty(outputPath)) outputPath = "Build/Standalone";

            var options = new BuildPlayerOptions
            {
                scenes = System.Array.ConvertAll(scenes, s => s.path),
                locationPathName = outputPath,
                target = EditorUserBuildSettings.activeBuildTarget,
                options = BuildOptions.None
            };

            var report = BuildPipeline.BuildPlayer(options);
            var summary = report.summary;
            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"[BuildTools] Build succeeded: {outputPath} (size {summary.totalSize} bytes).");
                EditorApplication.Exit(0);
            }
            else
            {
                Debug.LogError($"[BuildTools] Build failed: result={summary.result}, errors={summary.totalErrors}.");
                EditorApplication.Exit(1);
            }
        }
    }
}
