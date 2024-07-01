import { useEffect, useState } from "react";
import { useAuth } from "../context/AuthContext";
import { getNotFollowing, getPostsSeguidos, seguirUsuario, createPost } from "../api/auth";
import { Link } from "react-router-dom";

function WallPages() {
  const { usuario, setSelectedUsername } = useAuth(); // Agrega setSelectedUsername al contexto de autenticación

  const [usuarios, setUsuarios] = useState([]);
  const [posts, setPosts] = useState([]);
  const [newPostContent, setNewPostContent] = useState("");
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    if (usuario && usuario.usuarioId) {
      const fetchData = async () => {
        try {
          // Obtener usuarios que no son seguidos por el usuario actual
          const usuariosRes = await getNotFollowing(usuario.usuarioId);
          setUsuarios(usuariosRes);

          // Obtener posts de usuarios seguidos por el usuario actual
          const postsResponse = await getPostsSeguidos(usuario.usuarioId);
          setPosts(postsResponse.data);

          setIsLoading(false);
        } catch (error) {
          console.error("Error fetching data:", error);
          setIsLoading(false);
        }
      };

      fetchData();
    }
  }, [usuario]);

  const handleFollow = async (followeeId) => {
    try {
      setIsLoading(true);
      // Seguir al usuario seleccionado
      await seguirUsuario(usuario.usuarioId, followeeId);

      // Después de seguir, actualizar la lista de usuarios no seguidos
      const usuariosRes = await getNotFollowing(usuario.usuarioId);
      setUsuarios(usuariosRes);

      // Obtener de nuevo los posts de usuarios seguidos
      const postsResponse = await getPostsSeguidos(usuario.usuarioId);
      setPosts(postsResponse.data);

      setIsLoading(false);
    } catch (error) {
      console.error("Error following user:", error);
      setIsLoading(false);
    }
  };

  const handleCreatePost = async () => {
    try {
      const timestamp = new Date().toISOString();
      const newPost = {
        content: newPostContent,
        timestamp: timestamp,
        usuarioId: usuario.usuarioId,
      };

      // Crear un nuevo post
      await createPost(newPost);
      setNewPostContent("");
    } catch (error) {
      console.error("Error creating post:", error);
    }
  };

  const formatDate = (timestamp) => {
    const date = new Date(timestamp);
    return date.toLocaleString();
  };

  if (isLoading || !usuario || !usuario.usuarioId) {
    return <div>Loading...</div>;
  }

  return (
    <div className="bg-primary min-h-screen text-lightblue p-4">
      <h1 className="text-4xl font-bold mb-6 text-center text-blue">WallPages</h1>
      <div className="flex flex-col md:flex-row">
        <div className="md:w-1/4 bg-secondary p-4 rounded-lg shadow-md">
          <h2 className="text-2xl font-semibold mb-4 text-blue">Usuarios registrados</h2>
          <div className="space-y-4">
            {usuarios.map((user) => (
              <div key={user.usuarioId} className="flex justify-between items-center bg-secondary p-4 rounded-lg shadow-md">
                <Link
                  to={`/perfilUsuario/${user.usuarioId}`}
                  className="text-lightblue"
                  onClick={() => setSelectedUsername(user.usuario)} // Establecer el nombre de usuario seleccionado
                >
                  {user.usuario}
                </Link>
                <button
                  className="bg-blue text-primary py-1 px-4 rounded hover:bg-lightblue transition duration-300"
                  onClick={() => handleFollow(user.usuarioId)}
                >
                  Seguir
                </button>
              </div>
            ))}
          </div>
        </div>
        <div className="md:w-3/4 md:ml-4">
          <div className="mb-6">
            <h2 className="text-2xl font-semibold mb-4 text-blue">Crear nuevo post</h2>
            <textarea
              className="w-full p-2 mb-2 rounded text-primary"
              value={newPostContent}
              onChange={(e) => setNewPostContent(e.target.value)}
              placeholder="Escribe tu post aquí..."
            />
            <button
              className="bg-blue text-primary py-2 px-4 rounded hover:bg-lightblue transition duration-300"
              onClick={handleCreatePost}
            >
              Publicar
            </button>
          </div>
          <div>
            <h2 className="text-2xl font-semibold mb-4 text-blue">Posts de usuarios seguidos</h2>
            {posts.length === 0 ? (
              <p className="text-lightblue">No hay posts.</p>
            ) : (
              <div className="space-y-4">
                {posts.map((post) => (
                  <div key={post.postId} className="bg-secondary p-4 rounded-lg shadow-md">
                    <Link
                      to={`/perfilUsuario`}
                      className="text-lightblue"
                      onClick={() => setSelectedUsername(post.usuario)} // Establecer el nombre de usuario seleccionado
                    >
                      {post.usuario}
                    </Link>
                    <p className="text-lightblue">{post.content}</p>
                    <span className="text-sm text-blue">{formatDate(post.timestamp)}</span>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}

export default WallPages;
