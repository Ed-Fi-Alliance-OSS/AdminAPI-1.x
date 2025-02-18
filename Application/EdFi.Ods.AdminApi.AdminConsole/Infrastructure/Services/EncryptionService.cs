// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using static EdFi.Ods.AdminApi.AdminConsole.Helpers.Encryption;

namespace EdFi.Ods.AdminApi.AdminConsole.Infrastructure.Services;

public interface IEncryptionService
{
    bool TryEncrypt(string plainText, string encryptionKey, out string encryptedText);
    bool TryDecrypt(string encryptedText, string encryptionKey, out string decryptedText);
}

public class EncryptionService : IEncryptionService
{
    public bool TryEncrypt(string plainText, string encryptionKey, out string encryptedText)
    {
        encryptedText = string.Empty;
        if (string.IsNullOrEmpty(encryptionKey))
        {
            return false;
        }

        try
        {
            encryptedText = Encrypt(plainText, encryptionKey);
            return true;
        }
        catch (Exception ex)
        {
        }

        return false;
    }

    public bool TryDecrypt(string encryptedText, string encryptionKey, out string decryptedText)
    {
        decryptedText = string.Empty;
        if (string.IsNullOrEmpty(encryptionKey))
        {
            return false;
        }

        try
        {
            decryptedText = Decrypt(encryptedText, encryptionKey);
            return true;
        }
        catch (Exception ex)
        {
        }

        return false;
    }
}
