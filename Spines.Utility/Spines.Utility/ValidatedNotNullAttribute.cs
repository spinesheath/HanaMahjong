// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Spines.Utility
{
  /// <summary>
  /// If a parameter is marked with this Attribute, Code Analysis assumes that the paramter is checked for null.
  /// This does not perform the null check.
  /// </summary>
  internal sealed class ValidatedNotNullAttribute : Attribute
  {
  }
}