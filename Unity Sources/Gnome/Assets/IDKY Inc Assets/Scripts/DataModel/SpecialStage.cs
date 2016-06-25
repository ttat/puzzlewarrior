// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

namespace Idky
{
    using System.Runtime.Serialization;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SpecialStage
#if UNITY_IOS
        : ISerializable
#endif
    {
        #region Fields

        private string description;

        private LevelMapping levelMapping;

        private string stageId;

        #endregion

        #region Constructors and Destructors

        public SpecialStage()
        {
            this.levelMapping = new LevelMapping();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The text that will be displayed when setting up the stage.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
            }
        }

        public LevelMapping LevelMapping
        {
            get
            {
                return this.levelMapping;
            }
            set
            {
                this.levelMapping = value;
            }
        }

        /// <summary>
        /// Gets or sets the stage ID.  This should match the VirtualGood ID in the Store Assets.
        /// </summary>
        /// <value>
        /// The stage identifier.
        /// </value>
        public string StageId
        {
            get
            {
                return this.stageId;
            }
            set
            {
                this.stageId = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Deserialized()
        {
            this.OnDeserialized(new StreamingContext());
        }

#if UNITY_IOS
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("StageId", this.stageId);
            info.AddValue("Description", this.description);
        }
#endif

        public void Serializating()
        {
            this.OnSerializing(new StreamingContext());
        }

        #endregion

        #region Methods

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            this.levelMapping.Deserialized();
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            this.levelMapping.Serializating();
        }

        #endregion
    }
}