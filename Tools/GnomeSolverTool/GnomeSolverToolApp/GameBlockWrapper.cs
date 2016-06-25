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
    public class GameBlockWrapper : INotifyPropertyChanged
    {
        #region Fields

        private readonly IGameBlock gameBlock;

        private GameBoard.GameBlockType blockType;

        #endregion

        #region Constructors and Destructors

        public GameBlockWrapper(IGameBlock gameBlock)
        {
            this.gameBlock = gameBlock;

            if (this.gameBlock is GameBlockPlayer)
            {
                this.blockType = GameBoard.GameBlockType.Player;
            }
            else if (this.gameBlock is GameBlockNormal)
            {
                this.blockType = GameBoard.GameBlockType.Normal;
            }
            else if (this.gameBlock is GameBlockChangeDirection)
            {
                this.blockType = GameBoard.GameBlockType.ChangeDirection;
            }
            else if (this.gameBlock is GameBlockExtraMove)
            {
                this.blockType = GameBoard.GameBlockType.ExtraMove;
            }
            else if (this.gameBlock is GameBlockMultipleMoves)
            {
                this.blockType = GameBoard.GameBlockType.MultipleMoves;
            }
            else
            {
                this.blockType = GameBoard.GameBlockType.Null;
            }
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
                    this.NotifyPropertyChanged("Direction");
                }
            }
        }

        public MovementDirection Direction
        {
            get
            {
                GameBlockChangeDirection block = this.gameBlock as GameBlockChangeDirection;
                if (block != null)
                {
                    return block.ForceDirection;
                }
                else
                {
                    return MovementDirection.Down;
                }
            }
        }


        #endregion

        #region Public Methods and Operators

        public override string ToString()
        {
            string str;

            if (this.gameBlock is GameBlockPlayer)
            {
                str = string.Format("{0}:{1}", this.blockType, (this.gameBlock as GameBlockPlayer).AvailableMoves);
            }
            else if (this.gameBlock is GameBlockMultipleMoves)
            {
                str = (this.gameBlock as GameBlockMultipleMoves).NumberOfMovesNeeded.ToString();
            }
            else
            {
                str = string.Empty;
            }

            return str;
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