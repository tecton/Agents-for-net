// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Agents.Authentication.Msal.Model;

namespace Microsoft.Agents.Authentication.Msal.Interfaces
{
    public interface IMSALConnectionSettings : IConnectionSettings
    {
        public string ClientSecret { get; set; }

        AuthTypes AuthType { get; set; }

        string CertificateThumbPrint { get; set; }

        string CertificateSubjectName { get; set; }

        string CertificateStoreName { get; set; }

        public bool ValidCertificateOnly { get; set; }

        public bool SendX5C { get; set; }
    }
}