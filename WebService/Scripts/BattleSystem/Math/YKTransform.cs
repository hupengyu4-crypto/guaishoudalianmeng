using System;

namespace BattleSystem
{
    public class YKTransform
    {
        private double _mDirection;
        private YKVector3d _mPos;
        public YKTransform()
        {

        }
        public YKTransform(YKVector3d pos)
        {
            _mPos = pos;
        }

        public YKTransform(YKVector3d pos, double dir)
        {
            _mPos = pos;
            Direction = dir;
        }
        public YKTransform(YKVector3d pos, double dir, YKAABB3d bounds)
        {
            Bounds = bounds;
            Pos = pos;
            Direction = dir;
        }

        public YKAABB3d? Bounds { get; set; }

        /// <summary>
        /// eulerAngles  Y
        /// </summary>
        public double Direction
        {
            get => _mDirection;
            set
            {
                if (value > 180) value -= 360;
                _mDirection = value;
            }
        }

        public YKVector3d Pos
        {
            get => _mPos;
            set => _mPos = Bounds?.PointClamp(value) ?? value;
        }

        public YKVector3d Forward
        {
            get
            {
                var radian = Direction * (Math.PI / 180);
                return new YKVector3d(Math.Sin(radian), 0, Math.Cos(radian));
            }
        }

        public YKVector3d Right
        {
            get
            {
                var radian = (Direction + 90) * (Math.PI / 180);
                return new YKVector3d(Math.Sin(radian), 0, Math.Cos(radian));
            }
        }

        public void LookAt(YKTransform target)
        {
            LookAt(target.Pos);
        }

        public void LookAt(YKVector3d point)
        {
            Direction = YKVector3d.PointAngleXZ(Pos, point);
        }

        public void MoveForward(double delta)
        {
            Pos += Forward * delta;
        }

        public void MoveRight(double delta)
        {
            Pos += Right * delta;
        }

        /// <summary>
        /// 当前朝向的y和xz偏移
        /// </summary>
        /// <param name="offset"></param>
        public YKVector3d GetForward(YKVector3d offset)
        {
            var forward = Forward;
            forward.x *= offset.x;
            forward.z *= offset.z;
            forward.y = offset.y;
            return Pos + forward;
        }

        public YKVector3d GetForward(double offset)
        {
            return Pos + Forward * offset;
        }

        public void SetPosOutBounds(YKVector3d pos)
        {
            _mPos = pos;
        }
    }
}
