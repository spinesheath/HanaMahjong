// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Spines.Tools.AnalyzerBuilder
{
  internal interface IProgressManager
  {
    void Reset(int max);

    void Increment();

    void Done();
  }
}