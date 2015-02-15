// Spines.Tenhou.Client.INewState.cs
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
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Spines.Utility;

namespace Spines.Tenhou.Client.LocalServer.States
{
  internal class NewInMatchState : NewStateBase
  {
    private readonly LobbyConnection _connection;
    private readonly string _accountId;

    public NewInMatchState(LobbyConnection connection, string accountId)
    {
      _connection = connection;
      _accountId = accountId;
    }

    public override INewState Process(Message message)
    {
      RestartTimer();
      if (message.Content.Name == "BYE")
      {
        return new NewIdleState(_connection, _accountId);
      }
      _connection.MatchServer.ProcessMessage(_connection, message.Content);
      return this;
    }
  }

  internal class NewInQueueState : NewStateBase
  {
    private readonly LobbyConnection _connection;
    private readonly string _accountId;

    public NewInQueueState(LobbyConnection connection, string accountId)
    {
      _connection = connection;
      _accountId = accountId;
    }

    public override INewState Process(Message message)
    {
      RestartTimer();
      if (message.Content.Name != "JOIN")
      {
        return this;
      }
      var parts = message.Content.Attribute("t").Value.Split(new[] { ',' });
      var lobby = InvariantConvert.ToInt32(parts[0]);
      var matchType = new MatchType(InvariantConvert.ToInt32(parts[1]));
      if (_connection.MatchServer.IsInMatch(_connection, lobby, matchType))
      {
        _connection.MatchServer.ProcessMessage(_connection, message.Content);
        return new NewInMatchState(_connection, _accountId);
      }
      if (_connection.MatchServer.IsInQueue(_connection))
      {
        return this;
      }
      return new NewIdleState(_connection, _accountId);
    }
  }

  internal class NewIdleState : NewStateBase
  {
    private readonly LobbyConnection _connection;
    private readonly string _accountId;

    public NewIdleState(LobbyConnection connection, string accountId)
    {
      _connection = connection;
      _accountId = accountId;
    }

    public override INewState Process(Message message)
    {
      RestartTimer();
      if (message.Content.Name == "BYE")
      {
        return new NewConnectionEstablishedState(_connection);
      }
      if (message.Content.Name != "JOIN")
      {
        return this;
      }
      var parts = message.Content.Attribute("t").Value.Split(new[] { ',' });
      var lobby = InvariantConvert.ToInt32(parts[0]);
      var matchType = new MatchType(InvariantConvert.ToInt32(parts[1]));
      // TODO switch connection to accountId in matchQueue
      if (!_connection.MatchServer.CanEnterQueue(_connection, lobby, matchType))
      {
        return this;
      }
      _connection.MatchServer.EnterQueue(_connection, lobby, matchType);
      return new NewInQueueState(_connection, _accountId);
    }
  }

  internal class NewAuthenticatingState : NewStateBase
  {
    private readonly LobbyConnection _connection;
    private readonly string _accountId;

    public NewAuthenticatingState(LobbyConnection connection, string accountId)
    {
      _connection = connection;
      _accountId = accountId;
    }

    public override INewState Process(Message message)
    {
      RestartTimer();
      if (message.Content.Name != "AUTH")
      {
        return this;
      }
      var v = message.Content.Attribute("val").Value;
      if (_connection.AuthenticationService.IsValid(_accountId, v))
      {
        return new NewIdleState(_connection, _accountId);
      }
      return new NewFinalState();
    }
  }

  internal class NewConnectionEstablishedState : NewStateBase
  {
    private readonly LobbyConnection _connection;

    public NewConnectionEstablishedState(LobbyConnection connection)
    {
      _connection = connection;
    }

    public override INewState Process(Message message)
    {
      RestartTimer();
      if (message.Content.Name != "HELO")
      {
        // TODO Report spam
        return this;
      }
      var accountId = message.Content.Attribute("name").Value;
      if (!_connection.RegistrationService.IsRegistered(accountId))
      {
        return new NewFinalState();
      }
      LogOn(accountId);
      return new NewAuthenticatingState(_connection, accountId);
    }

    // TODO move this elsewhere, for example let authentication service create the message, or a translator class
    private void LogOn(string accountId)
    {
      var accountInformation = _connection.RegistrationService.GetAccountInformation(accountId);
      var authenticationString = _connection.AuthenticationService.GetAuthenticationString(accountId);
      var message = accountInformation.ToMessage();
      message.Add(new XAttribute("auth", authenticationString));
      _connection.SendToClient(message);
    }
  }

  internal interface INewState
  {
    bool IsFinal { get; }
    event EventHandler<StateTimedOutEventArgs> TimedOut;
    INewState Process(Message message);
  }

  internal sealed class NewFinalState : INewState
  {
    public bool IsFinal
    {
      get { return true; }
    }

    public INewState Process(Message message)
    {
      throw new InvalidOperationException("FinalState can't process any messages.");
    }

    public event EventHandler<StateTimedOutEventArgs> TimedOut
    {
      add { }
      remove { }
    }
  }

  internal abstract class NewStateBase : INewState
  {
    private readonly int _milliseconds;
    private readonly Stopwatch _stopwatch = new Stopwatch();

    protected NewStateBase()
      : this(5000)
    {
    }

    protected NewStateBase(int milliseconds)
    {
      _milliseconds = milliseconds;
      WaitForTimeOutAsync();
    }

    public bool IsFinal
    {
      get { return false; }
    }

    public abstract INewState Process(Message message);

    public event EventHandler<StateTimedOutEventArgs> TimedOut;

    protected virtual INewState GetTimedOutState()
    {
      return new NewFinalState();
    }

    protected void RestartTimer()
    {
      lock (_stopwatch)
      {
        _stopwatch.Restart();
      }
    }

    private void WaitForTimeOut()
    {
      while (true)
      {
        int remaining;
        lock (_stopwatch)
        {
          remaining = (int) (_milliseconds - _stopwatch.ElapsedMilliseconds);
        }
        if (remaining <= 0)
        {
          EventUtility.CheckAndRaise(TimedOut, this, new StateTimedOutEventArgs(GetTimedOutState()));
          break;
        }
        Thread.Sleep(remaining);
      }
    }

    private async void WaitForTimeOutAsync()
    {
      RestartTimer();
      await Task.Run(() => WaitForTimeOut());
    }
  }
}