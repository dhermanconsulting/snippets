param(

    [string] $InputString = "b65f2142d01fe706083173b064c04cfc6b81ab2417d39d63d2b3216176d0e638b89cbca0f1c4294db56b66668f94ff0f",
    [ValidateSet("ENCMTHD_3", "ENCMTHD_2", "ENCMTHD_1")]
    [string] $Mode = "ENCMTHD_3"

)

class AESCipher {
    [byte[]]$Key

    AESCipher([byte[]]$key) {
        $this.Key = $key
    }

    [byte[]]Decrypt([byte[]]$enc, [string]$mode) {
        $aes = New-Object System.Security.Cryptography.AesManaged
        $aes.Padding = [System.Security.Cryptography.PaddingMode]::PKCS7

        if ($mode -eq "ENCMTHD_2") {
            $aes.Mode = [System.Security.Cryptography.CipherMode]::ECB
        }
        elseif ($mode -eq "ENCMTHD_3") {
            $aes.Mode = [System.Security.Cryptography.CipherMode]::CBC
            $aes.IV = New-Object byte[] 16  # Zero initialization vector
        }
        else {
            Write-Host "Invalid mode"
            return $false
        }

        $aes.Key = $this.Key

        $decryptor = $aes.CreateDecryptor()
        $decrypted = $decryptor.TransformFinalBlock($enc, 0, $enc.Length)
        $aes.Dispose()

        return $decrypted
    }
}


function Convert-HexToByteArray {
    param (
        [string] $hexString
    )

    $bytes = New-Object byte[] ($hexString.Length / 2)
    for ($i = 0; $i -lt $bytes.Length; $i++) {
        $bytes[$i] = [Convert]::ToByte($hexString.Substring($i * 2, 2), 16)
    }

    return $bytes
}

######################
# SCRIPT ENTRY POINT #
######################

[byte[]]  $aesKey = Convert-HexToByteArray "351CBE38F041320F22D990AD8365889C7DE2FCCCAE5A1A8707E21E4ADCCD4AD9" # Built-In NetScaler Key
[byte[]] $rc4Key = Convert-HexToByteArray "2286da6ca015bcd9b7259753c2a5fbc2" # Built-In NetScaler Key
[byte[]] $cipherText = Convert-HexToByteArray $InputString

# $cipherText = [Convert]::FromBase64String($InputString)

if ($mode -eq "ENCMTHD_3" -or $mode -eq "ENCMTHD_2") {
    $cipher = [AESCipher]::new($aesKey)
    [byte[]]$decoded = $cipher.Decrypt($cipherText, $mode)
    if ($mode -eq "ENCMTHD_3") {

        [System.Text.Encoding]::UTF8.GetString($decoded[16..$decoded.Count])
    }
    else {
        # Untested
        [System.Text.Encoding]::UTF8.GetString($decoded[16..$decoded.Count])

    }
}
elseif ($mode -eq "ENCMTHD_1") {
    throw; # Can't test and probably won't work anyway
    # old rc4 mode
}
