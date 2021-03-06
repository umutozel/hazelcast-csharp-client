/*
* Copyright (c) 2008-2015, Hazelcast, Inc. All Rights Reserved.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

namespace Hazelcast.IO.Ssl
{
    internal class BasicSSLContextFactory : SSLContextFactory
    {
        //private const string JavaNetSslPrefix = "javax.net.ssl.";

        //private SSLContext sslContext;

        //public BasicSSLContextFactory()
        //{
        //}

        ///// <exception cref="System.Exception"></exception>
        //public virtual void Init(Properties properties)
        //{
        //    KeyStore ks = KeyStore.GetInstance("JKS");
        //    KeyStore ts = KeyStore.GetInstance("JKS");
        //    string keyStorePassword = GetProperty(properties, "keyStorePassword");
        //    string keyStore = GetProperty(properties, "keyStore");
        //    if (keyStore == null || keyStorePassword == null)
        //    {
        //        throw new RuntimeException("SSL is enabled but keyStore[Password] properties aren't set!");
        //    }
        //    string trustStore = GetProperty(properties, "trustStore", keyStore);
        //    string trustStorePassword = GetProperty(properties, "trustStorePassword", keyStorePassword);
        //    string keyManagerAlgorithm = properties.GetProperty("keyManagerAlgorithm", "SunX509");
        //    string trustManagerAlgorithm = properties.GetProperty("trustManagerAlgorithm", "SunX509");
        //    string protocol = properties.GetProperty("protocol", "TLS");
        //    char[] keyStorePassPhrase = keyStorePassword.ToCharArray();
        //    LoadKeyStore(ks, keyStorePassPhrase, keyStore);
        //    KeyManagerFactory kmf = KeyManagerFactory.GetInstance(keyManagerAlgorithm);
        //    kmf.Init(ks, keyStorePassPhrase);
        //    LoadKeyStore(ts, trustStorePassword.ToCharArray(), trustStore);
        //    TrustManagerFactory tmf = TrustManagerFactory.GetInstance(trustManagerAlgorithm);
        //    tmf.Init(ts);
        //    sslContext = SSLContext.GetInstance(protocol);
        //    sslContext.Init(kmf.GetKeyManagers(), tmf.GetTrustManagers(), null);
        //}

        ///// <exception cref="System.IO.IOException"></exception>
        ///// <exception cref="Hazelcast.Net.Ext.NoSuchAlgorithmException"></exception>
        ///// <exception cref="Hazelcast.Net.Ext.CertificateException"></exception>
        //private void LoadKeyStore(KeyStore ks, char[] passPhrase, string keyStoreFile)
        //{
        //    InputStream input = new FileInputStream(keyStoreFile);
        //    try
        //    {
        //        ks.Load(input, passPhrase);
        //    }
        //    finally
        //    {
        //        IOUtil.CloseResource(input);
        //    }
        //}

        //private static string GetProperty(Properties properties, string property)
        //{
        //    string value = properties.GetProperty(property);
        //    if (value == null)
        //    {
        //        value = properties.GetProperty(JavaNetSslPrefix + property);
        //    }
        //    if (value == null)
        //    {
        //        value = Runtime.GetProperty(JavaNetSslPrefix + property);
        //    }
        //    return value;
        //}

        //private static string GetProperty(Properties properties, string property, string defaultValue)
        //{
        //    string value = GetProperty(properties, property);
        //    return value != null ? value : defaultValue;
        //}

        //public virtual SSLContext GetSSLContext()
        //{
        //    return sslContext;
        //}
    }
}