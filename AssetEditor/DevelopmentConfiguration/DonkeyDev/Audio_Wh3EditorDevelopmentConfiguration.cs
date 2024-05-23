﻿using AssetEditor.UiCommands;
using Audio.Presentation.AudioExplorer;
using Shared.Core.Events;
using SharedCore;

namespace AssetEditor.DevelopmentConfiguration.DonkeyDev
{
    internal class Audio_Wh3EditorDevelopmentConfiguration : IDeveloperConfiguration
    {
        private readonly IUiCommandFactory _uiCommandFactory;

        public Audio_Wh3EditorDevelopmentConfiguration(IUiCommandFactory uiCommandFactory)
        {
            _uiCommandFactory = uiCommandFactory;
        }

        public string[] MachineNames => DonkeyMachineNameProvider.MachineNames;
        public bool IsEnabled => false;
        public void OverrideSettings(ApplicationSettings currentSettings)
        {
            currentSettings.CurrentGame = GameTypeEnum.Warhammer3;
            currentSettings.SkipLoadingWemFiles = false;
        }

        public void OpenFileOnLoad()
        {
            _uiCommandFactory.Create<OpenEditorCommand>().Execute<AudioEditorViewModel>();
        }
    }
}