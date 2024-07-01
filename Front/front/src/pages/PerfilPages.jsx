import { useEffect, useState } from "react";
import { useAuth } from "../context/AuthContext";
import { getUsuarioPosts, deletePost, getPost, updatePost, getFollowers, getFollowing, deleteUser } from "../api/auth";
import { Link, useNavigate } from "react-router-dom";

function PerfilPages() {
  const { usuario, logout } = useAuth(); // Asegúrate de tener acceso a la función de logout
  const navigate = useNavigate();
  const [userInfo, setUserInfo] = useState({
    usuario: "",
    seguidores: 0,
    siguiendo: 0,
  });
  const [userPosts, setUserPosts] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [deletePostId, setDeletePostId] = useState(null); // Estado para almacenar el postId que se eliminará
  const [editPost, setEditPost] = useState(null); // Estado para almacenar el post que se editará
  const [deleteProfile, setDeleteProfile] = useState(false); // Estado para la ventana de confirmación de eliminación de perfil

  useEffect(() => {
    const fetchData = async () => {
      try {
        // Simplemente configurar userInfo directamente desde usuario
        setUserInfo({
          usuario: usuario.usuario,
        });

        // Obtener los posts del usuario usando usuario.usuarioId
        const userPostsRes = await getUsuarioPosts(usuario.usuarioId);
        setUserPosts(userPostsRes);

        // Obtener seguidores y seguidos
        const followersRes = await getFollowers(usuario.usuarioId);
        const followingRes = await getFollowing(usuario.usuarioId);

        setUserInfo((prevUserInfo) => ({
          ...prevUserInfo,
          seguidores: followersRes.length,
          siguiendo: followingRes.length,
        }));

        setIsLoading(false);
      } catch (error) {
        console.error("Error fetching data:", error);
        setIsLoading(false);
      }
    };

    fetchData();
  }, [usuario.usuarioId]); // Usar usuario.usuarioId como dependencia

  const handleDeletePost = (postId) => {
    setDeletePostId(postId); // Almacenar el postId que se eliminará
  };

  const confirmDeletePost = async () => {
    try {
      await deletePost(deletePostId);
      const updatedPosts = userPosts.filter((post) => post.postId !== deletePostId);
      setUserPosts(updatedPosts);
      setDeletePostId(null); // Reiniciar el estado de deletePostId después de la eliminación exitosa
    } catch (error) {
      console.error("Error deleting post:", error);
    }
  };

  const cancelDeletePost = () => {
    setDeletePostId(null); // Reiniciar el estado de deletePostId si se cancela la eliminación
  };

  const handleEditPost = async (postId) => {
    try {
      const post = await getPost(postId);
      setEditPost(post); // Almacenar el post que se editará
    } catch (error) {
      console.error("Error fetching post details:", error);
    }
  };

  const confirmEditPost = async () => {
    try {
      await updatePost(editPost.postId, editPost); // Actualizar el post
      const updatedPosts = userPosts.map((post) =>
        post.postId === editPost.postId ? editPost : post
      );
      setUserPosts(updatedPosts);
      setEditPost(null); // Reiniciar el estado de editPost después de la edición exitosa
    } catch (error) {
      console.error("Error updating post:", error);
    }
  };

  const cancelEditPost = () => {
    setEditPost(null); // Reiniciar el estado de editPost si se cancela la edición
  };

  const handleDeleteProfile = () => {
    setDeleteProfile(true); // Mostrar ventana de confirmación de eliminación de perfil
  };

  const confirmDeleteProfile = async () => {
    try {
      await deleteUser(usuario.usuarioId);
      logout(); // Cerrar sesión después de eliminar el perfil
    } catch (error) {
      console.error("Error deleting profile:", error);
    }
  };

  const cancelDeleteProfile = () => {
    setDeleteProfile(false); // Ocultar ventana de confirmación de eliminación de perfil
  };

  const formatDate = (timestamp) => {
    const date = new Date(timestamp);
    return date.toLocaleString(); // Formato localizado de fecha y hora
  };

  if (isLoading) {
    return <div>Loading...</div>;
  }

  return (
    <div className="bg-primary min-h-screen text-lightblue p-4">
      <h1 className="text-4xl font-bold mb-6 text-center text-blue">Perfil de {userInfo.usuario}</h1>
      <div className="flex justify-between items-center mb-6">
        <div>
          <p className="text-xl font-semibold">Número de posts: {userPosts.length}</p>
          <Link to="/seguidores"><p className="text-xl font-semibold">Seguidores: {userInfo.seguidores}</p></Link>
          <Link to="/seguidores"><p className="text-xl font-semibold">Siguiendo: {userInfo.siguiendo}</p></Link>
        </div>
        <div>
          <button
            className="bg-blue text-primary py-2 px-4 rounded hover:bg-lightblue transition duration-300 mr-4"
            onClick={() => {
              navigate("/editarPerfil")
            }}
          >
            Actualizar perfil
          </button>

          <button
            className="bg-red-600 text-primary py-2 px-4 rounded hover:bg-red-700 transition duration-300"
            onClick={handleDeleteProfile}
          >
            Eliminar perfil
          </button>
        </div>
      </div>
      <div className="space-y-6">
        {userPosts.map((post) => (
          <div key={post.postId} className="bg-secondary p-4 rounded-lg shadow-md">
            <p className="text-xl font-semibold mb-2">{formatDate(post.timestamp)}</p>
            {editPost && editPost.postId === post.postId ? (
              <div>
                <textarea
                  className="w-full p-2 mb-2 text-primary"
                  value={editPost.content}
                  onChange={(e) => setEditPost({ ...editPost, content: e.target.value })}
                />
                <div className="flex justify-end mt-2">
                  <button
                    className="bg-blue text-primary py-1 px-4 rounded hover:bg-lightblue transition duration-300 mr-2"
                    onClick={confirmEditPost}
                  >
                    Aceptar
                  </button>
                  <button
                    className="bg-red-600 text-primary py-1 px-4 rounded hover:bg-red-700 transition duration-300"
                    onClick={cancelEditPost}
                  >
                    Cancelar
                  </button>
                </div>
              </div>
            ) : (
              <div>
                <p className="text-lightblue">{post.content}</p>
                <div className="flex justify-end mt-2">
                  <button
                    className="bg-blue text-primary py-1 px-4 rounded hover:bg-lightblue transition duration-300 mr-2"
                    onClick={() => handleEditPost(post.postId)}
                  >
                    Editar
                  </button>
                  <button
                    className="bg-red-600 text-primary py-1 px-4 rounded hover:bg-red-700 transition duration-300"
                    onClick={() => handleDeletePost(post.postId)}
                  >
                    Eliminar
                  </button>
                </div>
              </div>
            )}
          </div>
        ))}
      </div>

      {/* Modal de confirmación para eliminar post */}
      {deletePostId && (
        <div className="fixed inset-0 flex items-center justify-center bg-gray-500 bg-opacity-50 z-50">
          <div className="bg-white p-8 rounded-lg shadow-lg">
            <p className="text-xl font-semibold mb-4">¿Estás seguro que deseas eliminar este post?</p>
            <div className="flex justify-end">
              <button
                className="bg-red-600 text-primary py-2 px-4 rounded hover:bg-red-700 transition duration-300 mr-2"
                onClick={confirmDeletePost}
              >
                Confirmar
              </button>
              <button
                className="bg-blue text-primary py-2 px-4 rounded hover:bg-lightblue transition duration-300"
                onClick={cancelDeletePost}
              >
                Cancelar
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Modal de confirmación para eliminar perfil */}
      {deleteProfile && (
        <div className="fixed inset-0 flex items-center justify-center bg-gray-500 bg-opacity-50 z-50">
          <div className="bg-white p-8 rounded-lg shadow-lg">
            <p className="text-xl font-semibold mb-4">¿Estás seguro que deseas eliminar tu perfil?</p>
            <div className="flex justify-end">
              <button
                className="bg-red-600 text-primary py-2 px-4 rounded hover:bg-red-700 transition duration-300 mr-2"
                onClick={confirmDeleteProfile}
              >
                Confirmar
              </button>
              <button
                className="bg-blue text-primary py-2 px-4 rounded hover:bg-lightblue transition duration-300"
                onClick={cancelDeleteProfile}
              >
                Cancelar
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

export default PerfilPages;
