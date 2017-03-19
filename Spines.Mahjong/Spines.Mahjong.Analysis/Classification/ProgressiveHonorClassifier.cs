// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Spines.Mahjong.Analysis.Classification
{
  /// <summary>
  /// Returns the arrangement value of honors after the execution of a single action.
  /// </summary>
  internal struct ProgressiveHonorClassifier
  {
    /// <summary>
    /// Creates an instance of ProgressiveHonorClassifier with the same state as the current one.
    /// </summary>
    /// <returns>A new instance of ProgressiveHonorClassifier.</returns>
    public ProgressiveHonorClassifier Fork()
    {
      return new ProgressiveHonorClassifier { _current = _current };
    }

    public ProgressiveHonorClassifier Clone()
    {
      return new ProgressiveHonorClassifier {_current = _current};
    }

    public int Draw(int previousTiles, int melded)
    {
      var actionId = previousTiles + melded + (melded & 1);
      _current = Transitions[_current + actionId + 1];
      return Transitions[_current];
    }

    public int Discard(int tilesAfterDiscard, int melded)
    {
      var actionId = 6 + tilesAfterDiscard + melded + (melded & 1);
      _current = Transitions[_current + actionId];
      return Transitions[_current];
    }

    public int Pon(int previousTiles)
    {
      _current = Transitions[_current + previousTiles + 9];
      return Transitions[_current];
    }

    public int Daiminkan()
    {
      _current = Transitions[_current + 13];
      return Transitions[_current];
    }

    private static readonly ushort[] Transitions = Resource.Transitions("ProgressiveHonorStateMachine.txt");
    private int _current;
  }
}