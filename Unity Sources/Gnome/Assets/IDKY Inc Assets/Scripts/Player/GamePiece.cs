// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using System;
using System.Collections.Generic;

using Idky;

using UnityEngine;

public class GamePiece : MonoBehaviour
{
    #region Fields

    public IGameBlock GameBlock;

    public UILabel Text;

    private Dictionary<BlockAnimationType, GameObject> animationMappingsDictionary;

    private Queue<AnimationQueueContainer> animationQueue = new Queue<AnimationQueueContainer>();

    private int blockSize;

    private MovementDirection? lastDirection;

    private AnimationQueueContainer nextAnimation;

    #endregion

    #region Public Methods and Operators

    public bool ApplyMove(IGameBlockParent parentBlock, MovementDirection direction, out BlockMovement move)
    {
        if (this.GameBlock == null)
        {
            move = null;
            return false;
        }

        move = new BlockMovement(this.GameBlock as IGameBlockParent, direction);
        bool appliedMove = this.GameBlock.ApplyMove(parentBlock, direction, move);

        // Disable the TutorialAnimation once the move is applied
        foreach (Transform child in transform)
        {
            if (child.tag.Equals("TutorialAnimation"))
            {
                child.gameObject.SetActive(false);
            }
        }

        return appliedMove;
    }

    public void LoadImage()
    {
        if (this.GameBlock != null)
        {
            this.SetAnimation(this.animationMappingsDictionary[this.GameBlock.IdleAnimation], true, false);
            this.Text.text = this.GameBlock.Text;
        }
        else
        {
            this.ClearAnimation(true);
            this.Text.text = string.Empty;
        }
    }

    public void OnCancelMove()
    {
        IGameBlockParent gameBlockParent = this.GameBlock as IGameBlockParent;

        if (gameBlockParent != null)
        {
            this.LoadImage();
        }

        this.lastDirection = null;
    }

    public void OnMoving(MovementDirection direction, float pixels)
    {
        if (this.lastDirection == null || this.lastDirection != direction)
        {
            this.lastDirection = direction;
            IGameBlockParent gameBlockParent = this.GameBlock as IGameBlockParent;
            if (gameBlockParent != null)
            {
                if (gameBlockParent.AvailableMoves > 0)
                {
                    BlockAnimationType animationType = gameBlockParent.GetPreparingMoveAnimation(direction);
                    GameObject animationObject = this.animationMappingsDictionary[animationType];

                    this.SetAnimation(animationObject, true, false);
                }
            }
        }
    }

    public void SetAnimationMappings(Dictionary<BlockAnimationType, GameObject> mappings)
    {
        this.animationMappingsDictionary = mappings;
    }

    public void SetBlockSize(int size)
    {
        this.blockSize = size;

        UIWidget uiWidget = this.GetComponent<UIWidget>();

        if (uiWidget != null)
        {
            uiWidget.SetDimensions(size, size);
        }

        // Keep the font size about 1/3 of the block size is a good ratio for it to display correctly
        this.Text.fontSize = size / 3;
    }

    public void SubscribeToGameBlockEvents()
    {
        if (this.GameBlock != null)
        {
            this.GameBlock.GameBlockSetAnimationEvent += this.OnGameBlockSetAnimationEvent;
        }
    }

    #endregion

    #region Methods

    private bool ClearAnimation(bool canOverride)
    {
        bool cleared;

        Transform findChild = this.transform.FindChild("Animation");
        if (findChild != null)
        {
            UI2DSpriteAnimationIdky uiAnimation = findChild.GetComponent<UI2DSpriteAnimationIdky>();

            if (uiAnimation.AnimationEnded || canOverride)
            {
                Destroy(findChild.gameObject);
                cleared = true;
            }
            else
            {
                cleared = false;
            }
        }
        else
        {
            Debug.Log("Animation child is null");
            cleared = true;
        }

        return cleared;
    }

    private void OnGameBlockSetAnimationEvent(object sender, GameBlockSetAnimationEventArgs e)
    {
        GameObject animationObject = this.animationMappingsDictionary[e.AnimationType];

        Action action = () => this.SetAnimation(animationObject, e.CanOverride, e.DisableSounds);

        this.SendMessageUpwards("OnQueueAnimation", action, SendMessageOptions.DontRequireReceiver);

        this.Text.text = this.GameBlock != null ? this.GameBlock.Text : string.Empty;
    }

    private void OnPress(bool isPressed)
    {
        this.SendMessageUpwards(isPressed ? "GameBlockSelected" : "GameBlockUnselected", this);
    }

    private void SetAnimation(GameObject blockAnimation, bool canOverride, bool disableSounds)
    {
        if (canOverride)
        {
            this.ClearAnimation(true);
            this.animationQueue.Clear();
        }

        AnimationQueueContainer animationQueueContainer = new AnimationQueueContainer(blockAnimation, disableSounds);
        this.animationQueue.Enqueue(animationQueueContainer);
    }

    private void SetNewAnimation(GameObject blockAnimation, bool disableSounds)
    {
        GameObject childAnimation = NGUITools.AddChild(this.gameObject, blockAnimation);
        childAnimation.transform.localRotation = blockAnimation.transform.localRotation;
        childAnimation.name = "Animation";

        // Disable sounds if needed
        if (disableSounds)
        {
            SoundFxOnStart soundFxOnStart = childAnimation.GetComponent<SoundFxOnStart>();
            SoundFxOnStartRandom soundFxOnStartRandom = childAnimation.GetComponent<SoundFxOnStartRandom>();

            if (soundFxOnStart != null)
            {
                soundFxOnStart.DisableSounds = true;
            }

            if (soundFxOnStartRandom != null)
            {
                soundFxOnStartRandom.DisableSounds = true;
            }
        }

        UIWidget uiWidget = childAnimation.GetComponent<UIWidget>();

        if (uiWidget != null)
        {
            // Get the scaling factors
            int originalWidth = uiWidget.width;
            int originalHeight = uiWidget.height;
            float widthScalingFactor = (float)this.blockSize / originalWidth;
            float heightScalingFactor = (float)this.blockSize / originalHeight;

            uiWidget.SetDimensions(this.blockSize, this.blockSize);

            // Set Child FX sizes
            UIWidget[] widgetChildren = uiWidget.GetComponentsInChildren<UIWidget>();
            foreach (UIWidget widgetChild in widgetChildren)
            {
                // Only attempt to rescale if the child isn't anchored
                if (widgetChild != uiWidget && !widgetChild.isAnchored)
                {
                    // Scale the child with the same ratio as the parent
                    widgetChild.SetDimensions(
                        (int)(widthScalingFactor * widgetChild.width), (int)(heightScalingFactor * widgetChild.height));

                    // Also scale the position
                    Vector3 oldPosition = widgetChild.gameObject.transform.localPosition;
                    float newX = oldPosition.x * widthScalingFactor;
                    float newY = oldPosition.y * heightScalingFactor;

                    widgetChild.gameObject.transform.localPosition = new Vector3(newX, newY);
                }
            }
        }
    }

    private void Update()
    {
        if (this.nextAnimation != null)
        {
            if (this.ClearAnimation(false))
            {
                this.SetNewAnimation(this.nextAnimation.Animation, this.nextAnimation.DisableSounds);
                this.nextAnimation = null;
            }
        }
        else if (this.nextAnimation == null && this.animationQueue.Count > 0)
        {
            this.nextAnimation = this.animationQueue.Dequeue();

            if (this.ClearAnimation(false))
            {
                this.SetNewAnimation(this.nextAnimation.Animation, this.nextAnimation.DisableSounds);
                this.nextAnimation = null;
            }
        }
    }

    #endregion

    private class AnimationQueueContainer
    {
        #region Constructors and Destructors

        public AnimationQueueContainer(GameObject animation, bool disableSounds)
        {
            this.Animation = animation;
            this.DisableSounds = disableSounds;
        }

        #endregion

        #region Public Properties

        public GameObject Animation { get; set; }

        public bool DisableSounds { get; set; }

        #endregion
    }
}