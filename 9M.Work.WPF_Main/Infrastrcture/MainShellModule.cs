using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _9M.Work.WPF_Main.Infrastrcture
{
   public  class MainShellModule : IModule
    {
        private  IRegionManager _regionManager;
        public void Initialize(IRegionManager regionManager)
        {
            this._regionManager = regionManager;
        }
        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion(RegionNames.TopRegion,typeof(Views.TopView));
            _regionManager.RegisterViewWithRegion(RegionNames.LeftNavRegion, typeof(Views.LeftNavView));
            _regionManager.RegisterViewWithRegion(RegionNames.RightContentRegion, typeof(Views.RightContentView));
            _regionManager.RegisterViewWithRegion(RegionNames.BottomRegion, typeof(Views.BottomView));
        }

       
    }
}
