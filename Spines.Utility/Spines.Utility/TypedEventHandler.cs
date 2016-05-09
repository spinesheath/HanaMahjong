// Spines.Utility.TypedEventHandler.cs
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

namespace Spines.Utility
{
  /// <summary>
  /// Strongly typed event handler.
  /// </summary>
  /// <param name="sender">The sender of the event.</param>
  /// <param name="arguments">The arguments of the event.</param>
  /// <typeparam name="TSender">The type of the sender.</typeparam>
  /// <typeparam name="TEventArgs">The type of the arguments.</typeparam>
  public delegate void TypedEventHandler<in TSender, in TEventArgs>(TSender sender, TEventArgs arguments);
}