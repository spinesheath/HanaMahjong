// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Spines.Tools.AnalyzerBuilder.Classification
{
  internal class FinalState : State
  {
    public FinalState(int alphabetSize, int value)
      : base(alphabetSize)
    {
      Value = value;
    }

    public int Value { get; private set; }
  }
}