#include<stdio.h>
#include<stdlib.h>
int main()
{
	printf("�ڷ|���z�w�˹q���̯ʤ֪� Windows ��s : Framework 4.5\n");
	printf("I'll install the Windows Update not installed in your computer : Framework 4.5\n");
	printf("���������󨾬r�n��A�_�h�i��ɭP�w�˥���\n");
	printf("Please shut down any antivirus software or the installation may fail.\n");
	printf("���b�Ұ� Framework 4.5 �w�˵{���A�еy��......\n");
	printf("Launching Framework 4.5 installation, please wait......\n");
	printf("�Ш̷ӫ��ܶi��w�ˡA�w�˹L�{�i��ݭn10~30����\n");
	printf("Please follow the instructions to install, this procedure may take 10~30 mins to complete.\n");
	system("\"Debug\\dotNetFx45_Full_setup.exe\"");
	printf("�p�G�z���̷ӫ��ܦ��\\�w�˧�s�A�{�b�C�����ӥi�H����F\n");
	printf("If you follow the instructions and successfully complete the installation, the game is executable now.\n");
	printf("��������H�ҰʹC���A�ï��A�C���r��~~~\n");
	printf("Press any key to launch the game, have fun~~~\n");
	system("pause");
	system("start \"\" \"�}�l�C��.exe\"");
	system("exit");
	return 0;
}
