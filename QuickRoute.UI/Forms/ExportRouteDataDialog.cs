using System;
using System.Collections.Generic;
using System.Windows.Forms;
using QuickRoute.BusinessEntities.Exporters;
using QuickRoute.BusinessEntities.RouteProperties;

namespace QuickRoute.UI.Forms
{
  public partial class ExportRouteDataDialog : Form
  {
    private DateTime zeroTime;

    public ExportRouteDataDialog(ExportRouteDataSettings settings)
    {
      InitializeComponent();

      samplingIntervalDropdown.Text = settings.SamplingInterval.TotalSeconds.ToString();

      // (*) handling buggy multiple occurrences of route property types
      var routePropertyTypes = new List<Type>();


      // add the names and visibility status of the route properties
      foreach (var item in settings.RoutePropertyTypes)
      {
        if (!routePropertyTypes.Contains(item.RoutePropertyType)) // (*) 
        {
          routePropertyTypeCheckboxList.Items.Add(item, item.Selected);
          routePropertyTypes.Add(item.RoutePropertyType); // (*) 
        }
      }
      zeroTime = settings.ZeroTime; 
    }

    public ExportRouteDataSettings Settings
    {
      get
      {
        var ret = new ExportRouteDataSettings() { ZeroTime = zeroTime };
        double value;
        if (double.TryParse(samplingIntervalDropdown.Text, out value))
        {
          if (value <= 0) value = 1;
          if (value > 3600) value = 3600;
          ret.SamplingInterval = new TimeSpan((long)(TimeSpan.TicksPerSecond * value));
        }
        else
        {
          ret.SamplingInterval = new TimeSpan(0, 0, 1);
        }

        var srpt = new SelectableRoutePropertyTypeCollection();
        for (var i = 0; i < routePropertyTypeCheckboxList.Items.Count; i++)
        {
          var item = (SelectableRoutePropertyType)routePropertyTypeCheckboxList.Items[i];
          srpt.Add(new SelectableRoutePropertyType(item.RoutePropertyType, routePropertyTypeCheckboxList.GetItemChecked(i)));
        }
        ret.RoutePropertyTypes = srpt;

        return ret;
      }
    }
    
    private void ok_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.OK;
      Close();
    }

    private void cancel_Click(object sender, EventArgs e)
    {
      DialogResult = DialogResult.Cancel;
      Close();
    }

    private void samplingIntervalDropdown_Leave(object sender, EventArgs e)
    {
      double value;
      if (double.TryParse(samplingIntervalDropdown.Text, out value))
      {
        if (value <= 0) value = 1;
        if (value > 3600) value = 3600;
        samplingIntervalDropdown.Text = value.ToString();
      }
      else
      {
        samplingIntervalDropdown.Text = "1";
      }
    }
  }
}