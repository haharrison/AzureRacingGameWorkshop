#region File Description
//-----------------------------------------------------------------------------
// ChaseCamera.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using directives
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using RacingGame.GameLogic;
using RacingGame.Graphics;
using RacingGame.Helpers;
using RacingGame.Properties;
using Color = Microsoft.Xna.Framework.Color;
using Model = RacingGame.Graphics.Model;
using System.Diagnostics;
#endregion

namespace RacingGame.GameLogic
{
    /// <summary>
    /// Chase camera for our car. We are always close behind it.
    /// The camera rotation is not the same as the current car rotation,
    /// we interpolate the values a bit, allowing the user to do small changes
    /// without rotating the camera frantically. Also feels more realistic in
    /// curves. Derived from the CarController class, which controls the car
    /// by the user input. This camera class is not controlled by the user,
    /// its all automatic!
    /// </summary>
    public class ChaseCamera : CarPhysics
    {
        protected Vector3 cameraPos;
        private float cameraDistance;
        private Vector3 cameraLookVector;



        private List<List<TrackCamPosition>> TrackCams;


        public enum CameraMode
        {
            Default,
            FreeCamera,
            TrackCamera
        }

        public CameraMode Mode = CameraMode.Default;


        private Matrix rotMatrix = Matrix.Identity;


        private Vector3 wannaCameraLookVector = Vector3.Zero;
        private float wannaCameraDistance = 0.0f;

        //public Matrix RotationMatrix
        //{
        //    get
        //    {
        //        return rotMatrix;
        //    }
        //}
        

        
       
        const int MaxCameraWobbelTimeoutMs = 700;
        static float cameraWobbelTimeoutMs = 0;
        static float cameraWobbelFactor = 1.0f;

        public static void WobbelCamera(float wobbelFactor)
        {
            cameraWobbelTimeoutMs = MaxCameraWobbelTimeoutMs;
            cameraWobbelFactor = wobbelFactor;
        }
        


        //public Vector3 CameraPosition
        //{
        //    get
        //    {
        //        return cameraPos;
        //    }
        //}

        //static public Vector3 XAxis
        //{
        //    get
        //    {
        //        // Get x column
        //        return new Vector3(
        //            BaseGame.ViewMatrix.M11,
        //            BaseGame.ViewMatrix.M21,
        //            BaseGame.ViewMatrix.M31);
        //    }
        //}


        //static public Vector3 YAxis
        //{
        //    get
        //    {
        //        // Get y column
        //        return new Vector3(
        //            BaseGame.ViewMatrix.M12,
        //            BaseGame.ViewMatrix.M22,
        //            BaseGame.ViewMatrix.M32);
        //    }
        //}


        //static public Vector3 ZAxis
        //{
        //    get
        //    {
        //        // Get z column
        //        return new Vector3(
        //            BaseGame.ViewMatrix.M13,
        //            BaseGame.ViewMatrix.M23,
        //            BaseGame.ViewMatrix.M33);
        //    }
        //}

        //public bool FreeCamera
        //{
        //    get
        //    {
        //        return Mode == CameraMode.FreeCamera;
        //    }
        //    set
        //    {
        //        if (value == true)
        //            Mode = CameraMode.FreeCamera;
        //        else
        //            Mode = CameraMode.Default;
        //    }
        //}


        

        //public ChaseCamera(Vector3 setCarPosition, Vector3 setDirection, Vector3 setUp, Vector3 setCameraPos)
        //    : base(setCarPosition, setDirection, setUp)
        //{
        //    SetCameraPosition(setCameraPos);
        //}


        //public ChaseCamera(Vector3 setCarPosition, Vector3 setCameraPos)
        //    : base(setCarPosition)
        //{
        //    SetCameraPosition(setCameraPos);
        //}


        public ChaseCamera(Vector3 setCarPosition)
            : base(setCarPosition)
        {
            SetTrackCams();

            SetCameraPosition(setCarPosition + new Vector3(0, 10.0f, 25.0f));
        }

        private void SetTrackCams()
        {
            TrackCams = new List<List<TrackCamPosition>>();

            // Track 0
            List<TrackCamPosition> track0Cams = new List<TrackCamPosition>();

            track0Cams.Add(new TrackCamPosition(0, 1552, 917, 135));
            track0Cams.Add(new TrackCamPosition(35, 1396, 864, 100));
            track0Cams.Add(new TrackCamPosition(140, 1304, 748, 60));   // Bad
            track0Cams.Add(new TrackCamPosition(200, 1073, 394, 180));   // Bad


            track0Cams.Add(new TrackCamPosition(320, 1200, 428, 100));
            track0Cams.Add(new TrackCamPosition(500, 1604, 436, 80));
            track0Cams.Add(new TrackCamPosition(661, 1920, 416, 45)); // Good
            track0Cams.Add(new TrackCamPosition(800, 2176, 516, 100));
            track0Cams.Add(new TrackCamPosition(920, 2249, 781, 60));

            track0Cams.Add(new TrackCamPosition(1020, 2084, 1152, 50));

            track0Cams.Add(new TrackCamPosition(1330, 1856, 948, 180));



            TrackCams.Add(track0Cams);



        }


        

        public void SetCameraPosition(Vector3 setCameraPos)
        {
            //Debug.WriteLine("SetCameraPosition");


            //if (RacingGameManager.Player != null)
            //{
            //    Debug.WriteLine(RacingGameManager.Player.trackSegmentNumber + " - " + RacingGameManager.Player.CarPosition.ToString());
            //}

            if (Mode == CameraMode.TrackCamera)
            {
                //cameraPos = new Vector3(2000, 700, 200);
                //cameraPos = new Vector3(1388, 868, 80);


                int trackNr = RacingGameManager.Player.LevelNum;

                foreach (TrackCamPosition pos in TrackCams[RacingGameManager.Player.LevelNum])
                {
                    if (pos.StartSector < RacingGameManager.Player.trackSegmentNumber)
                    {
                        cameraPos = pos.Position;
                    }
                }
               

                cameraDistance = Vector3.Distance(LookAtPos, cameraPos);
                cameraLookVector = LookAtPos - cameraPos;
                wannaCameraDistance = cameraDistance;
                wannaCameraLookVector = cameraLookVector;



                // Build look at matrix
                rotMatrix = Matrix.CreateLookAt(cameraPos, LookAtPos, CarUpVector);

            }
            else
            {
                cameraPos = setCameraPos;
                cameraDistance = Vector3.Distance(LookAtPos, cameraPos);
                cameraLookVector = LookAtPos - cameraPos;
                wannaCameraDistance = cameraDistance;
                wannaCameraLookVector = cameraLookVector;



                // Build look at matrix
                rotMatrix = Matrix.CreateLookAt(cameraPos, LookAtPos, CarUpVector);
            }
        }




        public void InterpolateCameraPosition(Vector3 setInterpolatedCameraPos)
        {
            // Don't use for free camera
            //if (FreeCamera)
            //    return; 
            if (Mode != CameraMode.Default)
                return;

            if (wannaCameraDistance == 0.0f)
                SetCameraPosition(setInterpolatedCameraPos);

            wannaCameraDistance = Vector3.Distance(LookAtPos, setInterpolatedCameraPos);
            wannaCameraLookVector = LookAtPos - setInterpolatedCameraPos;
        }


        


        //private Vector3 freeCameraRot = new Vector3( MathHelper.Pi, 0, -MathHelper.Pi / 2);


        //Vector3 wannaHaveCameraRotation = Vector3.Zero;


        //private void HandleFreeCamera()
        //{
        //    // Don't control the camera in the menu or game, only in unit tests!
        //    if (cameraMode != CameraMode.FreeCamera)
        //        return;

        //    float rotationFactor = 0.0075f;
        //    float gamePadRotFactor = 5.0f * BaseGame.MoveFactorPerSecond;

        //    // We don't use lookDistance or cameraRotation here, so we have
        //    // to calculate this values here.
        //    cameraDistance = cameraLookVector.Length();

        //    if (wannaHaveCameraRotation.Equals(Vector3.Zero))
        //        wannaHaveCameraRotation = freeCameraRot;
        //    Vector3 rot = wannaHaveCameraRotation;

        //    float addRotX =
        //        // Allow mouse input
        //        -Input.MouseXMovement * rotationFactor +
        //        // And gamepad input
        //        Input.GamePad.ThumbSticks.Left.X * gamePadRotFactor;
        //    // Also allow gamepad and keyboard cursors
        //    if (addRotX == 0)
        //    {
        //        if (Input.GamePadLeftPressed ||
        //            Input.KeyboardLeftPressed)
        //            addRotX = -gamePadRotFactor;
        //        if (Input.GamePadRightPressed ||
        //            Input.KeyboardRightPressed)
        //            addRotX = +gamePadRotFactor;
        //    }
        //    float addRotY =
        //        // Allow mouse input
        //        -Input.MouseYMovement * rotationFactor +
        //        // And gamepad input
        //        Input.GamePad.ThumbSticks.Left.Y * gamePadRotFactor;
        //    // Also allow gamepad and keyboard cursors
        //    if (addRotY == 0)
        //    {
        //        if (Input.GamePadUpPressed ||
        //            Input.KeyboardUpPressed)
        //            addRotY = -gamePadRotFactor;
        //        if (Input.GamePadDownPressed ||
        //            Input.KeyboardDownPressed)
        //            addRotY = +gamePadRotFactor;
        //    }

        //    wannaHaveCameraRotation = new Vector3(
        //        rot.X,
        //        rot.Y + addRotY,
        //        rot.Z + addRotX);

        //    // Mix camera rotation slowly to wanna have rotation
        //    freeCameraRot = Vector3.Lerp(
        //        freeCameraRot, wannaHaveCameraRotation, 0.5f);

        //    #region fix the "up-rotaion" to 0-180 degrees //old: 180-360 degrees
        //    // Substract a very small value to make sure we never reach PI,
        //    // this causes the z rotation to mess everything else up ...
        //    float minRotationRange = BaseGame.Epsilon;
        //    float maxRotationRange = (float)Math.PI - BaseGame.Epsilon;
        //    if (freeCameraRot.X < minRotationRange)
        //    {
        //        freeCameraRot.X = minRotationRange;
        //    }
        //    else if (freeCameraRot.X > maxRotationRange)
        //    {
        //        freeCameraRot.X = maxRotationRange;
        //    }
        //    #endregion

        //    // Calculate cameraPos like in HandleLookPosCamera()
        //    cameraLookVector = new Vector3(0, 0, cameraDistance);
        //    cameraLookVector = Vector3.TransformNormal(cameraLookVector,
        //        Matrix.CreateRotationX(freeCameraRot.X) *
        //        Matrix.CreateRotationY(freeCameraRot.Y) *
        //        Matrix.CreateRotationZ(freeCameraRot.Z));

        //    float moveFactor =
        //        (Input.Keyboard.IsKeyDown(Keys.LeftShift) ? 20.0f : 40.0f) *
        //        BaseGame.MoveFactorPerSecond;
        //    float smallMoveFactor = moveFactor / 4.0f;

        //    float lookDistanceChange = 0.0f;
        //    // Page up/down or Home/End to zoom in and out.
        //    if (Input.Keyboard.IsKeyDown(Keys.PageUp))
        //        lookDistanceChange += moveFactor * 0.05f;
        //    if (Input.Keyboard.IsKeyDown(Keys.PageDown))
        //        lookDistanceChange -= moveFactor * 0.05f;
        //    if (Input.Keyboard.IsKeyDown(Keys.Home))
        //        lookDistanceChange += smallMoveFactor * 0.05f;
        //    if (Input.Keyboard.IsKeyDown(Keys.End))
        //        lookDistanceChange -= smallMoveFactor * 0.05f;

        //    // Also allow mouse wheel to zoom
        //    if (Input.MouseWheelDelta != 0)
        //    {
        //        lookDistanceChange =
        //            Input.MouseWheelDelta * BaseGame.MoveFactorPerSecond / 16.0f;
        //    }

        //    // Also allow gamepad to zoom
        //    if (Input.GamePad.ThumbSticks.Right.Y != 0)
        //    {
        //        lookDistanceChange =
        //            Input.GamePad.ThumbSticks.Right.Y * BaseGame.MoveFactorPerSecond;
        //    }

        //    if (lookDistanceChange != 0)
        //    {
        //        // Half zoom effect if shift is pressed
        //        if (Input.Keyboard.IsKeyDown(Keys.LeftShift))
        //            lookDistanceChange /= 2.0f;

        //        cameraDistance *= 1.0f - lookDistanceChange;
        //        if (cameraDistance < 1.0f)
        //            cameraDistance = 1.0f;

        //        // Calculate cameraPos like in HandleLookPosCamera()
        //        cameraLookVector = Vector3.TransformNormal(
        //            new Vector3(0, 0, cameraDistance),
        //            Matrix.CreateRotationX(freeCameraRot.X) *
        //            Matrix.CreateRotationY(freeCameraRot.Y) *
        //            Matrix.CreateRotationZ(freeCameraRot.Z));
        //    }

        //    // Make sure we use these new values and don't interpolate them back.
        //    wannaCameraDistance = cameraDistance;
        //    wannaCameraLookVector = cameraLookVector;
        //}
        

        
        Vector3 lastCameraWobble = Vector3.Zero;



        private void UpdateViewMatrix()
        {
            //Debug.WriteLine("UpdateViewMatrix");

            if (Mode == CameraMode.TrackCamera)
            {
                rotMatrix = Matrix.CreateLookAt(cameraPos, LookAtPos, new Vector3() { X = 0, Y = 0, Z = 1 });

                BaseGame.ViewMatrix = rotMatrix;

            }
            else
            {

                cameraDistance = cameraDistance * 0.9f + wannaCameraDistance * 0.1f;
                cameraLookVector = (cameraLookVector * 0.9f) + (wannaCameraLookVector * 0.1f);

                cameraPos = LookAtPos + cameraLookVector;

                rotMatrix = Matrix.CreateLookAt(cameraPos, LookAtPos, CarUpVector);

                // Is camera wobbeling?
                if (cameraWobbelTimeoutMs > 0)
                {
                    cameraWobbelTimeoutMs -= BaseGame.ElapsedTimeThisFrameInMilliseconds;
                    if (cameraWobbelTimeoutMs < 0)
                        cameraWobbelTimeoutMs = 0;
                }

                // Add camera shake if camera wobbel effect is on
                if (cameraWobbelTimeoutMs > 0 &&
                    // But only if not zooming in and if in game.
                    ZoomInTime <= StartGameZoomTimeMilliseconds)
                {
                    float effectStrength = 1.5f * cameraWobbelFactor *
                        (cameraWobbelTimeoutMs / (float)MaxCameraWobbelTimeoutMs);
                    // Interpolate, make wobbleing more smoooth than in Rocket Commander
                    lastCameraWobble =
                        lastCameraWobble * 0.9f +
                        RandomHelper.GetRandomVector3(
                        -effectStrength, +effectStrength) * 0.1f;
                    rotMatrix *= Matrix.CreateTranslation(lastCameraWobble);
                }

                // Just set view matrix
                BaseGame.ViewMatrix = rotMatrix;
            }
        }
       

       

        public override void Reset()
        {
            base.Reset();
            cameraWobbelFactor = 0;
        }


        public override void ClearVariablesForGameOver()
        {
            base.ClearVariablesForGameOver();
            cameraWobbelFactor = 0;
        }
      


        /// <summary>
        /// Update camera, should be called every frame to handle all the input.
        /// </summary>
        public override void Update()
        {
            base.Update();

            // Only allow control when zooming is finished.
            //HandleFreeCamera();

            UpdateViewMatrix();
        }

    }
}
