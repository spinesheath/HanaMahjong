// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Spines.Tools.AnalyzerBuilder.Precalculation
{
  /// <summary>
  /// Creates the data for Shanten Calculations from scratch.
  /// If any intermediate data is present already, that part of the creation is skipped.
  /// </summary>
  internal class Creator
  {
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
      creator.CreateProgressiveKokushiTransitions();

      creator.CreateProgressiveHonorTransitions();
      creator.CreateSuitSecondPhase();
      creator.CreateSuitFirstPhase();
      creator.CreateArrangementTransitions();
      creator.CreateHonorTransitions();
      creator.CreateSuitTransitions();
    }

    private readonly string _workingDirectory;
  }
}