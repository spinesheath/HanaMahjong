// Spines.Tools.AnalyzerBuilder.Creator.cs
// 
// Copyright (C) 2017  Johannes Heckl
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

namespace Spines.Tools.AnalyzerBuilder.Precalculation
{
  /// <summary>
  /// Creates the data for Shanten Calculations from scratch.
  /// If any intermediate data is present already, that part of the creation is skipped.
  /// </summary>
  internal class Creator
  {
    private readonly string _workingDirectory;

    /// <summary>
    /// Creates a new Instance of Creator.
    /// </summary>
    /// <param name="workingDirectory">The directory where intermediate results are stored.</param>
    public Creator(string workingDirectory)
    {
      _workingDirectory = workingDirectory;
    }

    /// <summary>
    /// Creates the data for Shanten Calculations from scratch.
    /// If any intermediate data is present already, that part of the creation is skipped.
    /// </summary>
    public void Create()
    {
      var creator = new TransitionsCreator(_workingDirectory);
      creator.CreateProgressiveHonorTransitions();
      creator.CreateArrangementTransitions();
      creator.CreateHonorTransitions();
      creator.CreateSuitTransitions();
    }
  }
}