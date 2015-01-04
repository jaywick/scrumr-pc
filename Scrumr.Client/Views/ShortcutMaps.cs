using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Scrumr.Client.Views
{
    public class ShortcutMaps
    {
        private Dictionary<KeyCombo, Action> _shortcuts = new Dictionary<KeyCombo,Action>();

        public void Add(ModifierKeys modifiers, Key key, Action action)
        {
            var combo = new KeyCombo(modifiers, key);

            if (_shortcuts.ContainsKey(combo))
                _shortcuts.Remove(combo);

            _shortcuts.Add(combo, action);
        }

        public void Process(ModifierKeys modifiers, Key key)
        {
            var combo = new KeyCombo(modifiers, key);

            if (!_shortcuts.ContainsKey(combo))
                return;

            var action = _shortcuts[combo];
            
            if (action != null)
                action.Invoke();
        }

        private struct KeyCombo
        {
            ModifierKeys Modifiers;
            Key Key;

            public KeyCombo(ModifierKeys modifiers, Key key)
            {
                Modifiers = modifiers;
                Key = key;
            }
        }
    }
}
