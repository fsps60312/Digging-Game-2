#include<stdio.h>
#include<stdlib.h>
int main()
{
	printf("我會幫您安裝電腦裡缺少的 Windows 更新 : Framework 4.5\n");
	printf("I'll install the Windows Update not installed in your computer : Framework 4.5\n");
	printf("請關閉任何防毒軟體，否則可能導致安裝失敗\n");
	printf("Please shut down any antivirus software or the installation may fail.\n");
	printf("正在啟動 Framework 4.5 安裝程式，請稍後......\n");
	printf("Launching Framework 4.5 installation, please wait......\n");
	printf("請依照指示進行安裝，安裝過程可能需要10~30分鐘\n");
	printf("Please follow the instructions to install, this procedure may take 10~30 mins to complete.\n");
	system("\"Debug\\dotNetFx45_Full_setup.exe\"");
	printf("如果您有依照指示成功\安裝更新，現在遊戲應該可以執行了\n");
	printf("If you follow the instructions and successfully complete the installation, the game is executable now.\n");
	printf("按任何鍵以啟動遊戲，並祝你遊戲愉快~~~\n");
	printf("Press any key to launch the game, have fun~~~\n");
	system("pause");
	system("start \"\" \"開始遊戲.exe\"");
	system("exit");
	return 0;
}
