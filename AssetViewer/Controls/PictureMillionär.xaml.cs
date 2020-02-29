using AssetViewer.Data;
using AssetViewer.Templates;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace AssetViewer.Controls {

  /// <summary>
  /// Interaktionslogik für PictureMillionär.xaml
  /// </summary>
  public partial class PictureMillionär : UserControl, INotifyPropertyChanged {

    #region Properties

    public TemplateAsset SelectedAsset {
      get { return (TemplateAsset)GetValue(SelectedAssetProperty); }
      set { SetValue(SelectedAssetProperty, value); }
    }

    public bool IsGameRunning { get; set; }

    public long Points {
      get { return _points; }
      set {
        if (_points != value) {
          _points = value;
          RaisePropertyChanged(nameof(Points));
        }
      }
    }

    public int RightAnswersInRow {
      get { return _rightAnswersInRow; }
      private set {
        if (_rightAnswersInRow != value) {
          _rightAnswersInRow = value;
          RaisePropertyChanged(nameof(RightAnswersInRow));
        }
      }
    }

    public int Lifes {
      get { return _lifes; }
      private set {
        if (_lifes != value) {
          _lifes = value;
          RaisePropertyChanged(nameof(Lifes));
        }
      }
    }

    public int CountAnswers {
      get { return _countAnswers; }
      set {
        if (_countAnswers != value) {
          _countAnswers = value;
          RaisePropertyChanged(nameof(CountAnswers));
        }
      }
    }

    public ObservableCollection<Choice> Choices { get; set; } = new ObservableCollection<Choice>();

    public LinearGradientBrush RarityBrush {
      get {
        var selection = this.SelectedAsset?.RarityType ?? "Common";
        switch (selection) {
          case "Uncommon":
            return new LinearGradientBrush(new GradientStopCollection {
              new GradientStop(Color.FromRgb(65, 89, 41), 0),
              //new GradientStop(Color.FromRgb(42, 44, 39), 0.8),
              //new GradientStop(Color.FromRgb(42, 44, 39), 1)
            }, 90);

          case "Rare":
            return new LinearGradientBrush(new GradientStopCollection {
              new GradientStop(Color.FromRgb(50, 60, 83), 0),
              //new GradientStop(Color.FromRgb(42, 44, 39), 0.8),
              //new GradientStop(Color.FromRgb(42, 44, 39), 1)
            }, 90);

          case "Epic":
            return new LinearGradientBrush(new GradientStopCollection {
              new GradientStop(Color.FromRgb(90, 65, 89), 0),
              //new GradientStop(Color.FromRgb(42, 44, 39), 0.8),
              //new GradientStop(Color.FromRgb(42, 44, 39), 1)
            }, 90);

          case "Legendary":
            return new LinearGradientBrush(new GradientStopCollection {
              new GradientStop(Color.FromRgb(98, 66, 46), 0),
              //new GradientStop(Color.FromRgb(42, 44, 39), 0.8),
              //new GradientStop(Color.FromRgb(42, 44, 39), 1)
            }, 90);

          default:
            return new LinearGradientBrush(new GradientStopCollection {
              new GradientStop(Color.FromRgb(126, 128, 125), 0),
              //new GradientStop(Color.FromRgb(42, 44, 39), 0.8),
              //new GradientStop(Color.FromRgb(42, 44, 39), 1)
            }, 90);
        }
      }
    }

    #endregion Properties

    #region Fields

    public static readonly DependencyProperty SelectedAssetProperty =
        DependencyProperty.Register("SelectedAsset", typeof(TemplateAsset), typeof(PictureMillionär), new PropertyMetadata(null, OnSelectedAssetChanged));

    public
    Random random = new Random();
    private long _points;
    private int _rightAnswersInRow;
    private int _lifes;
    private int _countAnswers = 4;

    #endregion Fields

    #region Constructors

    public PictureMillionär() {
      InitializeComponent();
      Possebilities = AssetProvider.Items.Values.Where(a => a.ExpeditionAttributes?.Any(e => e.Text.ID == 1910367263 || e.Text.ID == -481309083) ?? false).ToList();
      DataContext = this;
    }

    #endregion Constructors

    #region Events

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion Events

    #region Methods

    public void RaisePropertyChanged([CallerMemberName]string name = "") {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public void StartGame() {
      if (!IsGameRunning) {
        Points = 0;
        RightAnswersInRow = 0;
        Lifes = 2;
        buttongrid.Visibility = Visibility.Hidden;
        Quizgrid.Visibility = Visibility.Visible;
        IsGameRunning = true;
        NextQuestion();
      }
    }

    #endregion Methods

    private string Alphabet { get; set; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    private static void OnSelectedAssetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
      if (d is PictureMillionär card) {
        card.RaisePropertyChanged(nameof(RarityBrush));
        card.RaisePropertyChanged(nameof(SelectedAsset));
      }  
    }
    public List<TemplateAsset> Possebilities { get; set; }
    private void NextQuestion() {
      Choices.Clear();
      var index = random.Next(Possebilities.Count);
      SelectedAsset = Possebilities.ElementAt(index);
      int gender = 0;
      if (SelectedAsset.ExpeditionAttributes.FirstOrDefault(e=> e.Text.ID == 1910367263) is Upgrade) {
        gender = 1910367263;
      } 
      if (SelectedAsset.ExpeditionAttributes.FirstOrDefault(e=> e.Text.ID == -481309083) is Upgrade) {
        gender = -481309083;
      }
      var tempchoices = new List<string>();
      tempchoices.Add(SelectedAsset.Text.CurrentLang);
      var tempPossibilities = Possebilities.Where(p => p.RarityType == SelectedAsset.RarityType && p.ExpeditionAttributes.Any(u => u.Text.ID == gender)).ToList();
      if (tempPossibilities.Count < CountAnswers) {
        tempPossibilities = Possebilities.Where(p => p.RarityType == SelectedAsset.RarityType).ToList();
      }   
      if (tempPossibilities.Count < CountAnswers) {
        tempPossibilities = Possebilities.ToList();
      }
      while (tempchoices.Count < CountAnswers) {
        var asset = tempPossibilities.ElementAt(random.Next(tempPossibilities.Count));
        if (!tempchoices.Contains(asset.Text.CurrentLang) && asset.Text.CurrentLang != SelectedAsset.Text.CurrentLang && asset.Text.Icon.Filename != SelectedAsset.Text.Icon.Filename) {
          tempchoices.Add(asset.Text.CurrentLang);
        }
      }
      for (int i = 0; i < CountAnswers; i++) {
        var hit = tempchoices[random.Next(tempchoices.Count)];
        var choice = new Choice() { Answer = hit, Letter = Alphabet[i].ToString() };
        Choices.Add(choice);
        tempchoices.Remove(hit);
      }
    }

    private void Button_Click(object sender, RoutedEventArgs e) {
      StartGame();
    }

    private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
      if (sender is ListView lv) {
        if (lv.SelectedItem is Choice choice) {
          if (choice.Answer == SelectedAsset.Text.CurrentLang) {
            Points += CountAnswers + (CountAnswers * RightAnswersInRow);
            RightAnswersInRow++;
            NextQuestion();
          }
          else {
            lv.SelectedItem = null;
            RightAnswersInRow = 0;
            if (Lifes == 0) {
              GameOver();
            }
            else {
              Lifes--;
            }
          }
        }
      }
    }

    private void GameOver() {
      IsGameRunning = false;
      buttongrid.Visibility = Visibility.Visible;
      Quizgrid.Visibility = Visibility.Hidden;
    }

    private void Button_Click_1(object sender, RoutedEventArgs e) {
      if (CountAnswers - 1 >= 2) {
        CountAnswers--;
      }
    }

    private void Button_Click_2(object sender, RoutedEventArgs e) {
        CountAnswers++;
    }
  }
}