﻿using CommonControls.Editors.AudioEditor.BnkCompiler;
using CommonControls.Services;
using FluentValidation;
using SharpDX.WIC;
using System.Collections.Generic;
using System.Linq;

namespace Audio.BnkCompiler.Validation
{




    public class AudioProjectXmlValidator : AbstractValidator<AudioInputProject>
    {
        public AudioProjectXmlValidator(PackFileService pfs, AudioInputProject projectXml)
        {
            var allItems = GetAllItems(projectXml);
            
            RuleFor(x => x).NotNull().WithMessage("Project file is missing");

            RuleFor(x => x.ProjectSettings).SetValidator(new SettingsValidator());
            RuleFor(x => x.GameSounds).ForEach(x => x.SetValidator(new GameSoundValidator(pfs)));
            RuleFor(x => x.Actions).ForEach(x => x.SetValidator(new ActionValidator(allItems)));
            RuleFor(x => x.Events).ForEach(x => x.SetValidator(new EventValidator(allItems)));
            
            // Validate that all ids are Uniqe
            RuleFor(x => x).Custom((projectFile, context) => ValidateUniqeIds(projectXml, context));
        }

        void ValidateUniqeIds(AudioInputProject projectXml, ValidationContext<AudioInputProject> context)
        {
            var allIds = GetAllItems(projectXml)
             .Select(x => x.Id)
             .ToList();

            var query = allIds.GroupBy(x => x)
              .Where(g => g.Count() > 1)
              .Select(y => new { Element = y.Key, Counter = y.Count() })
              .ToList();

            var duplicates = query.Where(x => x.Counter != 1).ToList();
            foreach (var duplicate in duplicates)
                context.AddFailure("Duplicate key", $"{duplicate.Element} is used {duplicate.Counter} times. Ids must be unique");
        }

        // Unreferenced check

        List<IAudioProjectHircItem> GetAllItems(AudioInputProject projectXml)
        {
            var output = new List<IAudioProjectHircItem>();
            output.AddRange(projectXml.Actions);
            output.AddRange(projectXml.Events);
            output.AddRange(projectXml.GameSounds);
            return output;
        }
    }
}