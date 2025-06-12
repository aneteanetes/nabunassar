using Geranium.Reflection;
using Microsoft.Xna.Framework;
using Nabunassar.Monogame.Interfaces;
using Nabunassar.Monogame.SpriteBatch;
using System.Reflection;

namespace Nabunassar.ECS
{
    //internal class ESCWorld : IRenderable
    //{
    //    public bool Enabled => true;

    //    public int UpdateOrder => 1;

    //    public event EventHandler<EventArgs> EnabledChanged;
    //    public event EventHandler<EventArgs> UpdateOrderChanged;

    //    private List<IUpdateable> _updatables = new();
    //    private List<IRenderable> _renderables = new();
    //    private List<ILoadable> _loadables = new();
    //    private List<ILoadable> _loaded = new();

    //    public void Update(GameTime gameTime)
    //    {
    //        foreach (var _updatable in _updatables)
    //        {
    //            if (_updatable.Enabled)
    //                _updatable.Update(gameTime);
    //        }
    //    }

    //    public void Add(object component)
    //    {
    //        if (component is IUpdateable updateable)
    //            _updatables.Add(updateable);

    //        bool isLoaded = false;
    //        if (component is IRenderable renderable)
    //        {
    //            isLoaded = true;
    //            AddLoadable(renderable);
    //        }

    //        if (!isLoaded)
    //            AddLoadable(component);
    //    }

    //    private void AddLoadable(object component)
    //    {
    //        if (component is ILoadable loadable)
    //        {
    //            if (_loaded.Contains(loadable))
    //            {
    //                loadable.Dispose();
    //                _loaded.Remove(loadable);
    //            }

    //            _loadables.Add(loadable);
    //        }
    //    }

    //    public void LoadContent()
    //    {
    //        foreach (var loadable in _loadables)
    //        {
    //            loadable.LoadContent();
    //            _loaded.Add(loadable);

    //            if(loadable is IRenderable renderable)
    //                _renderables.Add(renderable);
    //        }

    //        _loadables.Clear();
    //    }

    //    public void UnloadContent() 
    //        => Dispose();

    //    public void Draw(GameTime gameTime, SpriteBatchKnowed spriteBatch)
    //    {
    //        foreach (var renderable in _renderables)
    //        {
    //            if (renderable.Enabled)
    //                renderable.Draw(gameTime, spriteBatch);
    //        }
    //    }

    //    public void Dispose()
    //    {
    //        _updatables.ClearAndDispose();
    //        _renderables.ClearAndDispose();
    //        _loadables.ClearAndDispose();
    //        _loaded.ClearAndDispose();
    //    }
    //}
}
