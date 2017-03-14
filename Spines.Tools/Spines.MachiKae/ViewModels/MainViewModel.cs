// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Spines.Mahjong.Analysis.Classification;

namespace Spines.MachiKae.ViewModels
{
  internal class MainViewModel : ViewModelBase
  {
    public string Hand
    {
      get { return _hand; }
      set
      {
        if (value == _hand)
        {
          return;
        }
        _hand = value;
        try
        {
          _currentHand = new Hand(value);
        }
        catch
        {
          _currentHand = new Hand();
        }
        OnPropertyChanged();
        OnPropertyChanged(nameof(Shanten));
      }
    }

    public int Shanten
    {
      get
      {
        if (_currentHand == null || !_currentHand.IsValid)
        {
          return 8;
        }
        return _currentHand.Shanten;
      }
    }

    private string _hand;

    private Hand _currentHand;
  }
}