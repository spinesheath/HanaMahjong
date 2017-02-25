﻿// Spines.Tools.AnalyzerBuilder.TransitionsCreator.cs
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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Spines.Mahjong.Analysis.Classification;

namespace Spines.Tools.AnalyzerBuilder.Precalculation
{
  internal class TransitionsCreator
  {
    private readonly string _workingDirectory;

    /// <summary>
    /// Creates a new Instance of TransitionsCreator.
    /// </summary>
    /// <param name="workingDirectory">The directory where intermediate results are stored.</param>
    public TransitionsCreator(string workingDirectory)
    {
      _workingDirectory = workingDirectory;
    }

    public void CreateArrangementTransitions()
    {
      CreateTransitions("ArrangementTransitions.txt", GetArrangementBuilder);
    }

    public void CreateSuitTransitions()
    {
      CreateTransitions("SuitTransitions.txt", GetSuitBuilder);
    }

    public void CreateHonorTransitions()
    {
      CreateTransitions("HonorTransitions.txt", GetHonorBuilder);
    }

    public void CreateProgressiveHonorTransitions()
    {
      CreateTransitions("ProgressiveHonorStateMachine.txt", GetProgressiveHonorBuilder);
    }

    public void CreateProgressiveSuitTransitions()
    {
      CreateTransitions("ProgressiveSuitStateMachine.txt", GetProgressiveSuitBuilder);
    }

    private IStateMachineBuilder GetArrangementBuilder()
    {
      var language = new ArrangementWordCreator(_workingDirectory).CreateOrdered();
      return GetClassifierBuilder(language);
    }
    
    private IStateMachineBuilder GetSuitBuilder()
    {
      var language = new CompactAnalyzedDataCreator(_workingDirectory).CreateSuitWords();
      return GetClassifierBuilder(language);
    }

    private IStateMachineBuilder GetHonorBuilder()
    {
      var language = new CompactAnalyzedDataCreator(_workingDirectory).CreateHonorWords();
      return GetClassifierBuilder(language);
    }

    private IStateMachineBuilder GetProgressiveHonorBuilder()
    {
      var words = new CompactAnalyzedDataCreator(_workingDirectory).CreateHonorWords();
      var builder = new ProgressiveHonorStateMachineBuilder();
      builder.SetLanguage(words);
      return builder;
    }

    private IStateMachineBuilder GetProgressiveSuitBuilder()
    {
      var words = new CompactAnalyzedDataCreator(_workingDirectory).CreateSuitWords();
      var builder = new ProgressiveSuitStateMachineBuilder();
      builder.SetLanguage(words);
      return builder;
    }

    /// <summary>
    /// Creates the transitions file if it doesn't exist.
    /// </summary>
    private void CreateTransitions(string fileName, Func<IStateMachineBuilder> createBuilder)
    {
      var targetPath = Path.Combine(_workingDirectory, fileName);
      if (File.Exists(targetPath))
      {
        return;
      }

      var builder = createBuilder();
      var transitions = Transition.Compact(builder);

      var lines = transitions.Select(t => t.ToString(CultureInfo.InvariantCulture));
      File.WriteAllLines(targetPath, lines);
    }

    private static IStateMachineBuilder GetClassifierBuilder(IEnumerable<WordWithValue> language)
    {
      var builder = new ClassifierBuilder();
      builder.SetLanguage(language);
      return builder;
    }
  }
}