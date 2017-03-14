// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
        OnPropertyChanged();
      }
    }

    private string _hand;
  }
}