using System.Diagnostics;

namespace OWorkbench.Logics
{
    class BNetDetector
    {
        public static void CheckForBattleNet()
        {
            Process[] bnets = Process.GetProcessesByName("Battle.net");
            if (!(bnets == null || bnets.Length == 0))
                ToastManager.GetInstance().CreateToast("Battle.net is Running",
                    "Detected Battle.net client running. Running DataTool without shutting down Battle.net risks corrupting the client.", 2);
        }
    }
}
