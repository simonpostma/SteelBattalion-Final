; The name of the installer
outfile "SBC_installer_v3.0.1.exe"


; The default installation directory
InstallDir $PROGRAMFILES\SBC

; The text to prompt the user to enter a directory
;DirText "This will install My Cool Program on your computer. Choose a directory"

;--------------------------------
; The stuff to install
Section "" ;No components page, name is not important

; Set output path to the installation directory.
SetOutPath $INSTDIR
SectionEnd

; These are the programs that are needed by SBC Suite.
Section -Prerequisites
	File "vcredist_x86.exe"
	File "vJoySetup.exe"
	File "wdi-simple.exe"
	File "dotNetFx40_Full_setup.exe"
MessageBox MB_YESNO "Install prequisites for Steel Batallion Controller 64 bit driver?" /SD IDYES IDNO endREQ	
	ExecWait "vcredist_x86.exe"
	ExecWait "vJoySetup.exe";using new installer from 1/23/16
	ExecWait "wdi-simple.exe"
	ExecWait "dotNetFx40_Full_setup.exe"
	Goto endREQ
endREQ:	
SectionEnd