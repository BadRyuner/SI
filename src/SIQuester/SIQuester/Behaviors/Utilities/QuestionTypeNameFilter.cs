﻿namespace SIQuester.Utilities
{
    public sealed class QuestionTypeNameFilter: ICollectionFilter
    {
        public void Filter(object sender, System.Windows.Data.FilterEventArgs e)
        {
            e.Accepted = (e.Item as string).Length > 0;
        }
    }
}
