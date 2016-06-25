// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

namespace GnomeSolverToolApp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    using Idky;

    using Xceed.Wpf.DataGrid;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields

        private MainWindowViewModel viewModel;

        #endregion

        #region Constructors and Destructors

        public MainWindow()
        {
            this.InitializeComponent();
            this.viewModel = new MainWindowViewModel();
            this.viewModel.RefreshDataGridEvent += this.ViewModelOnRefreshDataGridEvent;

            this.DataContext = this.viewModel;
        }

        #endregion

        #region Methods

        private void ArchiveLevelsButtonClick(object sender, RoutedEventArgs e)
        {
            this.viewModel.CommandArchiveLevel.Execute(this.LoadGameBoardsDataGrid.SelectedItems.OfType<GameBoard>());
        }

        private void ArchiveLevelsButtonClickSpecialStages(object sender, RoutedEventArgs e)
        {
            this.viewModel.CommandArchiveLevel.Execute(this.LoadGameBoardsDataGridSpecialStages.SelectedItems.OfType<GameBoard>());
        }

        private void DeleteLevelsButtonClick(object sender, RoutedEventArgs e)
        {
            this.viewModel.CommandDeleteLevels.Execute(this.LoadGameBoardsDataGridArchivedLevels.SelectedItems.OfType<GameBoard>());
        }

        private void GameBlockButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (button != null)
            {
                GameBlockSelector gameBlockWrapper = (GameBlockSelector)button.DataContext;

                switch (gameBlockWrapper.BlockType)
                {
                    case GameBoard.GameBlockType.Null:
                        gameBlockWrapper.BlockType = GameBoard.GameBlockType.Normal;
                        break;

                    case GameBoard.GameBlockType.Normal:
                        gameBlockWrapper.BlockType = GameBoard.GameBlockType.Player;
                        break;

                    case GameBoard.GameBlockType.Player:
                        gameBlockWrapper.BlockType = GameBoard.GameBlockType.ChangeDirection;
                        gameBlockWrapper.Direction = MovementDirection.Up;
                        break;

                    case GameBoard.GameBlockType.ChangeDirection:
                        switch (gameBlockWrapper.Direction)
                        {
                            case MovementDirection.Up:
                                gameBlockWrapper.Direction = MovementDirection.Down;
                                break;
                            case MovementDirection.Down:
                                gameBlockWrapper.Direction = MovementDirection.Left;
                                break;
                            case MovementDirection.Left:
                                gameBlockWrapper.Direction = MovementDirection.Right;
                                break;
                            case MovementDirection.Right:
                                gameBlockWrapper.BlockType = GameBoard.GameBlockType.ExtraMove;
                                break;
                        }
                        break;

                    case GameBoard.GameBlockType.ExtraMove:
                        gameBlockWrapper.BlockType = GameBoard.GameBlockType.MultipleMoves;
                        gameBlockWrapper.NumberOfTimes = 2;
                        break;

                    case GameBoard.GameBlockType.MultipleMoves:
                        if (gameBlockWrapper.NumberOfTimes < 6)
                        {
                            gameBlockWrapper.NumberOfTimes++;
                        }
                        else
                        {
                            gameBlockWrapper.BlockType = GameBoard.GameBlockType.Null;
                        }
                        break;
                }
            }
        }

        private void GameBlockRightButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (button != null)
            {
                GameBlockSelector gameBlockWrapper = (GameBlockSelector)button.DataContext;

                gameBlockWrapper.BlockType = GameBoard.GameBlockType.Null;
            }
        }

        private void UnarchiveLevelsButtonClick(object sender, RoutedEventArgs e)
        {
            this.viewModel.CommandUnarchiveLevels.Execute(this.LoadGameBoardsDataGridArchivedLevels.SelectedItems.OfType<GameBoard>());
        }

        private void UnmapMappingButtonClick(object sender, RoutedEventArgs e)
        {
            List<KeyValuePair<int, string>> selectedMappings = new List<KeyValuePair<int, string>>();

            foreach (KeyValuePair<int, string> selectedItem in this.LevelMappingDataGrid.SelectedItems)
            {
                selectedMappings.Add(selectedItem);
            }

            this.viewModel.CommandRemoveLevelMapping.Execute(selectedMappings);
        }

        private void UnmapSpecialStageMappingButtonClick(object sender, RoutedEventArgs e)
        {
            List<KeyValuePair<int, string>> selectedMappings = new List<KeyValuePair<int, string>>();

            foreach (KeyValuePair<int, string> selectedItem in this.SpecialStagesLevelMappingDataGrid.SelectedItems)
            {
                selectedMappings.Add(selectedItem);
            }

            this.viewModel.CommandRemoveSpecialStageLevelMapping.Execute(selectedMappings);
        }

        private void ViewModelOnRefreshDataGridEvent(object sender, EventArgs eventArgs)
        {
            DataGridCollectionView lmCollectionView = this.LevelMappingDataGrid.ItemsSource as DataGridCollectionView;
            if (lmCollectionView != null)
            {
                lmCollectionView.Refresh();
            }

            DataGridCollectionView sslCollectionView = this.SpecialStagesLevelMappingDataGrid.ItemsSource as DataGridCollectionView;
            if (sslCollectionView != null)
            {
                sslCollectionView.Refresh();
            }

            DataGridCollectionView lgbCollectionView = this.LoadGameBoardsDataGrid.ItemsSource as DataGridCollectionView;
            if (lgbCollectionView != null)
            {
                lgbCollectionView.Refresh();
            }

            DataGridCollectionView lgbssCollectionView = this.LoadGameBoardsDataGridSpecialStages.ItemsSource as DataGridCollectionView;
            if (lgbssCollectionView != null)
            {
                lgbssCollectionView.Refresh();
            }

            DataGridCollectionView lgbalCollectionView = this.LoadGameBoardsDataGridArchivedLevels.ItemsSource as DataGridCollectionView;
            if (lgbalCollectionView != null)
            {
                lgbalCollectionView.Refresh();
            }
        }

        #endregion
    }
}