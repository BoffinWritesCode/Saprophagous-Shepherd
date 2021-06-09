using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MiniJam61Egypt.GameContent.Entities;
using MiniJam61Egypt.Util;

using BoffXNA.Base;

namespace MiniJam61Egypt.Physics
{
    public class PhysicsManager
    {
        //Static bodies are stored in chunks, as to reduce the number of checks made.
        private Dictionary<Point, List<Rectangle>> _staticBodies;

        private List<Entity> _entities;

        private int _chunkSize;

        private TimeManager _timeManager;

        public PhysicsManager(int chunkSize, TimeManager timeManager)
        {
            _staticBodies = new Dictionary<Point, List<Rectangle>>();
            _entities = new List<Entity>();
            _timeManager = timeManager;
            _chunkSize = chunkSize;
        }

        public void AddBody(Rectangle rect)
        {
            for (float x = rect.X; x < rect.Right; x += _chunkSize)
            {
                for (float y = rect.Y; y < rect.Bottom; y += _chunkSize)
                {
                    Point chunk = new Point((int)Math.Floor(x / _chunkSize), (int)Math.Floor(y / _chunkSize));
                    if (!_staticBodies.ContainsKey(chunk))
                    {
                        _staticBodies[chunk] = new List<Rectangle>();
                    }
                    _staticBodies[chunk].Add(rect);
                }
            }
        }

        public void ClearBodies()
        {
            _staticBodies.Clear();
        }

        public void AddEntity(Entity entity)
        {
            _entities.Add(entity);
        }

        public void RemoveEntity(Entity entity)
        {
            _entities.Remove(entity);
        }

        public void ClearEntities()
        {
            _entities.Clear();
        }

        public void UpdateMe(Entity entity, List<Rectangle> extras = null)
        {
            //Static entities can be moved by other entities, but don't move themselves
            if (entity.Static) return;

            if (!entity.HitsWalls)
            {
                entity.Body.Center += entity.Velocity * _timeManager.DeltaTime;
                return;
            }

            CheckMove(entity, entity.Velocity * _timeManager.DeltaTime, extras);
        }

        private bool CheckMove(Entity entity, Vector2 v, List<Rectangle> extraBodies = null)
        {
            float minMove = 1f;
            float xMove = Math.Sign(v.X);
            float yMove = Math.Sign(v.Y);
            float x1 = Math.Abs(v.X);
            float y1 = Math.Abs(v.Y);

            //Move horizontally first
            Point chunk = CoordinateToChunk(entity.Body.Center.ToPoint());
            List<Rectangle> bodies = GetStaticBodies(chunk.X, chunk.Y);
            if (extraBodies != null)
            {
                bodies.AddRange(extraBodies);
            }
            //bodies = bodies.OrderByDescending(body => body.TileShape).ToList();
            bodies = bodies.OrderByDescending(body => Vector2.Distance(body.Center.ToVector2(), entity.Body.Center)).ToList();
            bool didCollide = false;

            while (x1 > 0f)
            {
                float move = x1 >= minMove ? xMove * minMove : xMove * x1;

                entity.Body.X += move;
                bool collided = false;
                Rectangle? collisionBody = null;
                BoundingBox2D colRect = BoundingBox2D.Empty;

                foreach (Rectangle body in bodies)
                {
                    if (entity.Body.Intersects(body))
                    {
                        collisionBody = body;
                        collided = true;
                        didCollide = true;
                        break;
                    }
                }
                if (!entity.Solid)
                {
                    foreach (Entity e in _entities)
                    {
                        if (e.Solid)
                        {
                            if (entity.Body.Intersects(e.Body))
                            {
                                colRect = entity.Body;
                                didCollide = true;
                                collided = true;
                                break;
                            }
                        }
                    }
                }

                if (collided)
                {
                    bool end = false;
                    if (collisionBody.HasValue)
                    {
                        BoundingBox2D pen = entity.Body.Interpenetration(new BoundingBox2D(collisionBody.Value));
                        entity.Body.X -= xMove * pen.Width;
                        entity.OnHorizontalCollide();
                    }
                    else
                    {
                        BoundingBox2D pen = entity.Body.Interpenetration(colRect);
                        entity.Body.X -= xMove * pen.Width;
                        entity.OnHorizontalCollide();
                    }

                    if (end)
                    {
                        x1 = 0f;
                        break;
                    }
                }

                else if (entity.Solid)
                {
                    foreach (Entity e in _entities)
                    {
                        if (!e.Solid && e.InContactWith(entity))
                        {
                            if (CheckMove(e, Vector2.UnitX * move))
                            {
                                e.OnSquish();
                            }
                        }
                    }
                }

                x1--;
            }

            while (y1 > 0f)
            {
                float move = y1 >= minMove ? yMove : yMove * y1;

                entity.Body.Y += move;
                bool collided = false;
                Rectangle? collisionBody = null;
                BoundingBox2D colRect = BoundingBox2D.Empty;

                foreach (Rectangle body in bodies)
                {
                    if (entity.Body.Intersects(body))
                    {
                        collisionBody = body;
                        collided = true;
                        didCollide = true;
                        break;
                    }
                }
                if (!entity.Solid)
                {
                    foreach (Entity e in _entities)
                    {
                        if (e.Solid)
                        {
                            if (entity.Body.Intersects(e.Body))
                            {
                                colRect = entity.Body;
                                didCollide = true;
                                collided = true;
                                break;
                            }
                        }
                    }
                }

                if (collided)
                {
                    bool end = true;
                    if (collisionBody.HasValue)
                    {
                        BoundingBox2D pen = entity.Body.Interpenetration(new BoundingBox2D(collisionBody.Value));
                        entity.Body.Y -= yMove * pen.Height;
                        entity.OnVerticalCollide();
                    }
                    else
                    {
                        BoundingBox2D pen = entity.Body.Interpenetration(colRect);
                        entity.Body.Y -= yMove * pen.Height;
                        entity.OnVerticalCollide();
                    }

                    if (end)
                    {
                        y1 = 0f;
                        break;
                    }
                }
                else if (entity.Solid)
                {
                    foreach (Entity e in _entities)
                    {
                        if (!e.Solid && e.InContactWith(entity))
                        {
                            if (CheckMove(e, Vector2.UnitY * move))
                            {
                                e.OnSquish();
                            }
                        }
                    }
                }

                y1--;
            }
            return didCollide;
        }

        private List<Rectangle> GetStaticBodies(int x, int y)
        {
            List<Rectangle> bodies = GetChunk(x, y);
            if (bodies == null)
            {
                bodies = new List<Rectangle>();
            }
            else
            {
                bodies = bodies.ToList();
            }

            for (int chunkX = x - 1; chunkX <= x + 1; chunkX++)
            {
                for (int chunkY = y - 1; chunkY <= y + 1; chunkY++)
                {
                    if (chunkX == x && chunkY == y) continue;

                    List<Rectangle> chunk = GetChunk(chunkX, chunkY);
                    if (chunk != null)
                    {
                        foreach (Rectangle body in chunk)
                        {
                            bodies.Add(body);
                        }
                    }
                }
            }

            return bodies;
        }

        private Point CoordinateToChunk(Point coord)
        {
            return new Point(coord.X / _chunkSize, coord.Y / _chunkSize);
        }

        private List<Rectangle> GetChunk(int x, int y)
        {
            Point chunk = new Point(x, y);
            if (_staticBodies.TryGetValue(chunk, out List<Rectangle> list))
            {
                return list;
            }
            return null;
        }

        public void DebugDrawStatics(SpriteBatch spriteBatch, Entity entity, Point camera)
        {
            Point chunk = CoordinateToChunk(entity.Body.Center.ToPoint());
            List<Rectangle> bodies = GetStaticBodies(chunk.X, chunk.Y);

            foreach(Rectangle rect in bodies)
            {
                //spriteBatch.DrawRectangleDebug(new Rectangle(rect.X - camera.X, rect.Y - camera.Y, rect.Width, rect.Height), Color.Red, 1f, Main.Pixel);
            }
        }
    }
}
