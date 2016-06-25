// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

namespace GnomeSolverToolApp
{
    using System.ComponentModel;

    using Idky;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GameBlockSelector : INotifyPropertyChanged
    {
        #region Fields

        private GameBoard.GameBlockType blockType;

        private int column;

        private MovementDirection direction;

        private int numberOfTimes;

        private int preferredMax;

        private int row;

        #endregion

        #region Constructors and Destructors

        public GameBlockSelector(GameBoard.GameBlockType blockType)
        {
            this.blockType = blockType;
        }

        #endregion

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Properties

        public GameBoard.GameBlockType BlockType
        {
            get
            {
                return this.blockType;
            }
            set
            {
                if (this.blockType != value)
                {
                    this.blockType = value;
                    this.NotifyPropertyChanged("BlockType");
                    this.NotifyPropertyChanged("ImagePath");
                    this.NotifyPropertyChanged("Direction");
                    this.NotifyPropertyChanged("Text");
                }
            }
        }

        public int Column
        {
            get
            {
                return this.column;
            }
            set
            {
                if (this.column != value)
                {
                    this.column = value;
                    this.NotifyPropertyChanged("Column");
                }
            }
        }

        public MovementDirection Direction
        {
            get
            {
                return this.direction;
            }
            set
            {
                if (this.direction != value)
                {
                    this.direction = value;
                    this.NotifyPropertyChanged("Direction");
                    this.NotifyPropertyChanged("Text");
                }
            }
        }

        public int NumberOfTimes
        {
            get
            {
                return this.numberOfTimes;
            }
            set
            {
                if (this.numberOfTimes != value)
                {
                    this.numberOfTimes = value;
                    this.NotifyPropertyChanged("NumberOfTimes");
                    this.NotifyPropertyChanged("Text");
                }
            }
        }

        public int PreferredMax
        {
            get
            {
                return this.preferredMax;
            }
            set
            {
                if (this.preferredMax != value)
                {
                    this.preferredMax = value;
                    this.NotifyPropertyChanged("PreferredMax");
                }
            }
        }

        public int Row
        {
            get
            {
                return this.row;
            }
            set
            {
                if (this.row != value)
                {
                    this.row = value;
                    this.NotifyPropertyChanged("Row");
                }
            }
        }

        public string Text
        {
            get
            {
                string str;

                if (this.blockType == GameBoard.GameBlockType.ChangeDirection)
                {
                    str = this.direction.ToString();
                }
                else if (this.blockType == GameBoard.GameBlockType.MultipleMoves)
                {
                    str = this.numberOfTimes.ToString();
                }
                else
                {
                    str = this.blockType.ToString();
                }

                return str;
            }
        }

        #endregion

        #region Methods

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}