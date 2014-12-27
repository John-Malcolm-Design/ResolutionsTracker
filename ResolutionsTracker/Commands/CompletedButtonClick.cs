using ResolutionsTracker.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ResolutionsTracker.Commands
{
    class CompletedButtonClick : ICommand
    {
        public bool CanExecute(object parameter)
        {
            //throw new NotImplementedException();
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            //throw new NotImplementedException();

            // Call CompleteResolutionToday() method
            App.DataModel.CompleteResolutionToday((Resolution)parameter);

        }
    }
}
