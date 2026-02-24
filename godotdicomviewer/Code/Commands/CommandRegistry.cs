using System;
using System.Collections.Generic;
using Serilog;
using GodotDicomViewer.GUI.Controls;

namespace GodotDicomViewer.Code.Commands
{
    /// <summary>
    /// Centralized registry that maps command IDs to command objects.
    /// Replaces simple string lookups with proper object instantiation.
    /// </summary>
    public class CommandRegistry
    {
        private static readonly ILogger _log = Log.ForContext<CommandRegistry>();
        private readonly Dictionary<string, Func<ICommand>> _commandFactories = new();
        private readonly Dictionary<string, ICommand> _cachedCommands = new();

        public CommandRegistry()
        {
            // Register all available commands
            // File operations
            RegisterCommand<ExitCommand>();
            
            // View operations
            RegisterCommand<ZoomCommand>();
            RegisterCommand<ZoomInCommand>();
            RegisterCommand<AdjustWindowLevelCommand>();
            RegisterCommand<MeasureCommand>();
            
            // Data operations
            RegisterCommand<LoadStudyCommand>();
        }

        /// <summary>
        /// Register a command type. Will be instantiated on-demand.
        /// </summary>
        public void RegisterCommand<T>() where T : ICommand, new()
        {
            var command = new T();
            RegisterCommand(command.CommandID, () => new T());
        }

        /// <summary>
        /// Register a command with a factory function.
        /// </summary>
        public void RegisterCommand(string commandID, Func<ICommand> factory)
        {
            if (string.IsNullOrEmpty(commandID))
            {
                _log.Error("Cannot register command with empty ID");
                return;
            }

            _commandFactories[commandID] = factory;
            _cachedCommands.Remove(commandID);  // Clear cache if re-registering
            _log.Information("Registered command: {id}", commandID);
        }

        /// <summary>
        /// Get a command by ID. Returns cached instance.
        /// </summary>
        public ICommand? GetCommand(string commandID)
        {
            if (string.IsNullOrEmpty(commandID))
            {
                _log.Warning("GetCommand called with empty ID");
                return null;
            }

            // Return cached command if available
            if (_cachedCommands.TryGetValue(commandID, out var cached))
            {
                return cached;
            }

            // Create new instance from factory
            if (_commandFactories.TryGetValue(commandID, out var factory))
            {
                var command = factory();
                _cachedCommands[commandID] = command;
                return command;
            }

            _log.Warning("Command not found: {id}", commandID);
            return null;
        }

        /// <summary>
        /// Get all registered commands (useful for dynamic UI generation).
        /// </summary>
        public IEnumerable<ICommand> GetAllCommands()
        {
            var allCommands = new List<ICommand>();
            foreach (var commandID in _commandFactories.Keys)
            {
                var cmd = GetCommand(commandID);
                if (cmd != null)
                {
                    allCommands.Add(cmd);
                }
            }
            return allCommands;
        }

        /// <summary>
        /// Get commands by category (for organizing menus).
        /// </summary>
        public IEnumerable<ICommand> GetCommandsByCategory(string category)
        {
            var categoryCommands = new List<ICommand>();
            foreach (var command in GetAllCommands())
            {
                if (command.Category == category)
                {
                    categoryCommands.Add(command);
                }
            }
            return categoryCommands;
        }

        public bool HasCommand(string commandID)
        {
            return _commandFactories.ContainsKey(commandID);
        }
    }
}
