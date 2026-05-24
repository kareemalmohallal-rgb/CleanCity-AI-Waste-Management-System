import '../error/failures.dart';

sealed class AppResult<T> {
  const AppResult();
  R fold<R>(R Function(Failure f) onFail, R Function(T v) onOk);
}

final class Ok<T> extends AppResult<T> {
  final T value;
  const Ok(this.value);

  @override
  R fold<R>(R Function(Failure f) onFail, R Function(T v) onOk) => onOk(value);
}

final class Fail<T> extends AppResult<T> {
  final Failure failure;
  const Fail(this.failure);

  @override
  R fold<R>(R Function(Failure f) onFail, R Function(T v) onOk) => onFail(failure);
}
