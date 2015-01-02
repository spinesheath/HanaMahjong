// Spines.Utility.ValidationExtensions.cs
// 
// Copyright (C) 2015  Johannes Heckl
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;

namespace Spines.Utility
{
  /// <summary>
  /// Extension Methods for validating parameters.
  /// </summary>
  public static class ValidationExtensions
  {
    /// <summary>
    /// Throws an ArgumentNullException if the tested instance of a reference type is null.
    /// </summary>
    /// <typeparam name="T">The type of the instance to be tested.</typeparam>
    /// <param name="validatee">The instance to be tested for null.</param>
    /// <param name="argumentName">The name of argument that is to be tested.</param>
    public static void ThrowIfNull<T>([ValidatedNotNull] this T validatee, string argumentName) where T : class
    {
      if (validatee == null)
      {
        throw new ArgumentNullException(argumentName);
      }
    }
  }
}