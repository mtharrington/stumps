﻿namespace Stumps.Utility {

    using System.Security.Cryptography;
    using System.Text;

    internal static class RandomGenerator {

        public static char[] RandomCharacters = new char[36] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h',
                                                               'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p',
                                                               'q', 'r', 's', 't', 'u', 'v', 'w', 'x',
                                                               'y', 'z', '1', '2', '3', '4', '5', '6',
                                                               '7', '8', '9', '0' };

        public const int KeySize = 7;

        public static string GenerateIdentifier() {

            var identifier = string.Empty;

            using ( var cryptoProvider = new RNGCryptoServiceProvider() ) {

                var data = new byte[RandomGenerator.KeySize];
                cryptoProvider.GetNonZeroBytes(data);

                var sb = new StringBuilder();
                for ( int i = 0; i < RandomGenerator.KeySize; i++ ) {
                    sb.Append(RandomGenerator.RandomCharacters[data[i] % 36]);
                }

                identifier = sb.ToString();

            }

            return identifier;

        }

    }

}