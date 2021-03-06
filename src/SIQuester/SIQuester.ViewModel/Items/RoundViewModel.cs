﻿using SIPackages;
using SIQuester.Model;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SIQuester.ViewModel
{
    public sealed class RoundViewModel: ItemViewModel<Round>
    {
        public PackageViewModel OwnerPackage { get; set; }

        public override IItemViewModel Owner => OwnerPackage;

        public ObservableCollection<ThemeViewModel> Themes { get; } = new ObservableCollection<ThemeViewModel>();
        public override ICommand Add { get; protected set; }
        public override ICommand Remove { get; protected set; }
        public ICommand Clone { get; private set; }
        public ICommand AddTheme { get; private set; }

        public RoundViewModel(Round round)
            : base(round)
        {
            foreach (var theme in round.Themes)
            {
                Themes.Add(new ThemeViewModel(theme) { OwnerRound = this });
            }

            Themes.CollectionChanged += Themes_CollectionChanged;

            Clone = new SimpleCommand(CloneRound_Executed);
            Remove = new SimpleCommand(RemoveRound_Executed);
            Add = AddTheme = new SimpleCommand(AddTheme_Executed);
        }

        private void Themes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    for (int i = e.NewStartingIndex; i < e.NewStartingIndex + e.NewItems.Count; i++)
                    {
                        if (Themes[i].OwnerRound != null)
                            throw new Exception("Попытка вставить привязанную тему!");

                        Themes[i].OwnerRound = this;
                        Model.Themes.Insert(i, Themes[i].Model);
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (ThemeViewModel theme in e.OldItems)
                    {
                        theme.OwnerRound = null;
                        Model.Themes.RemoveAt(e.OldStartingIndex);

                        OwnerPackage.Document.ClearLinks(theme);
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    Model.Themes.Clear();
                    foreach (ThemeViewModel question in Themes)
                    {
                        question.OwnerRound = this;
                        Model.Themes.Add(question.Model);
                    }
                    break;
            }
        }

        private void CloneRound_Executed(object arg)
        {
            var newRound = Model.Clone() as Round;
            var newRoundViewModel = new RoundViewModel(newRound);
            OwnerPackage.Rounds.Add(newRoundViewModel);
            OwnerPackage.Document.Navigate.Execute(newRoundViewModel);
        }

        private void RemoveRound_Executed(object arg)
        {
            if (OwnerPackage == null)
            {
                return;
            }

            OwnerPackage.Rounds.Remove(this);
        }

        private void AddTheme_Executed(object arg)
        {
            try
            {
                var document = OwnerPackage.Document;

                var theme = new Theme { Name = "" };
                var themeViewModel = new ThemeViewModel(theme);

                document.BeginChange();

                try
                {
                    Themes.Add(themeViewModel);
                    if (AppSettings.Default.CreateQuestionsWithTheme)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            var question = QDocument.CreateQuestion((i + 1) * AppSettings.Default.QuestionBase);
                            themeViewModel.Questions.Add(new QuestionViewModel(question));
                        }
                    }
                }
                finally
                {
                    document.CommitChange();
                }

                QDocument.ActivatedObject = themeViewModel;
                document.Navigate.Execute(themeViewModel);
            }
            catch (Exception exc)
            {
                PlatformSpecific.PlatformManager.Instance.Inform(exc.Message, true);
            }
        }

        protected override void UpdateCosts(CostSetter costSetter)
        {
            var document = OwnerPackage.Document;

            document.BeginChange();
            try
            {
                base.UpdateCosts(costSetter);

                foreach (var th in Themes)
                {
                    th.UpdateCostsCore(costSetter);
                }
            }
            finally
            {
                document.CommitChange();
            }
        }
    }
}
